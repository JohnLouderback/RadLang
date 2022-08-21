using System.Runtime.CompilerServices;
using LLVMSharp.Interop;
using RadCompiler.Utils;
using RadParser;
using RadParser.AST.Node;
using RadParser.Utils;
using Void = RadParser.AST.Node.Void;

namespace RadCompiler;

public class CodeGenASTVisitor : BaseASTVisitor<LLVMBuilderRef> {
  private readonly LLVMModuleRef module;

  private readonly LLVMBuilderRef builder;

  private readonly LLVMContextRef context;

  /// <summary>
  ///   A weak map of AST nodes and their associated LLVM values references.
  /// </summary>
  public ConditionalWeakTable<INode, StrongBox<LLVMValueRef>> ASTValueMap { get; } = new();


  public CodeGenASTVisitor(LLVMModuleRef module, LLVMBuilderRef builder, LLVMContextRef context) {
    this.module  = module;
    this.builder = builder;
    this.context = context;
  }


  public override LLVMBuilderRef Visit(BinaryOperation node) {
    // Get a value of some sort out of the operands.
    var GetValue = new Func<Value, object>(
        possibleValue => {
          return possibleValue.Value switch {
            // If the value is a numeric value, return that number.
            NumericLiteral literal => literal.Value,

            // If the value is a identifier reference, get the declaration for the identifier.
            ReferenceExpression referenceExp => referenceExp.Reference.GetDeclaration(),

            // If this is a binary operation, return it so we can get the result later.
            BinaryOperation binOp => binOp
          };
        }
      );

    var GetLLVMValueRef = new Func<object, LLVMValueRef>(
        value => {
          // If the value is an integer, create an int constant.
          if (value is int intVal) {
            return LLVMValueRef.CreateConstInt(
                LLVMTypeRef.Int32,
                (ulong)
                intVal
              );
          }

          // If the value a reference to a function parameter.
          if (value is NamedTypeParameter param &&
              param.Parent is FunctionDeclaration function) {
            ASTValueMap.TryGetValue(function, out var val);
            var llvmFunc = val.Value;
            return llvmFunc.GetParam(
                (uint)function.Parameters.FindIndex(
                    parameter =>

                      // Find the parameter in the function's param list that matches this param node.
                      parameter == param
                  )
              );
          }

          // If the value is a binary operation expression.
          if (value is BinaryOperation binOp) {
            StrongBox<LLVMValueRef> llvmValue;

            // Try to get the LLVM value ref for the binary operation expression.
            if (!ASTValueMap.TryGetValue(binOp, out llvmValue)) {
              // If we couldn't get the value, get to visit that AST node now, to do it.
              Visit(binOp);

              // We should have the value now. Use that value.
              ASTValueMap.TryGetValue(binOp, out llvmValue);
            }

            return llvmValue.Value;
          }

          return default;
        }
      );

    var leftValue  = GetLLVMValueRef(GetValue(node.LeftOperand));
    var rightValue = GetLLVMValueRef(GetValue(node.RightOperand));

    // Add value returned by the math operations to the AST value map.
    ASTValueMap.Add(
        node,
        new StrongBox<LLVMValueRef>(
            node.Operator.Type switch {
              OperatorType.Plus => builder.BuildAdd(leftValue, rightValue),
              OperatorType.Minus => builder.BuildSub(leftValue, rightValue),
              OperatorType.Star => builder.BuildMul(leftValue, rightValue),
              OperatorType.ForwardSlash => builder.BuildSDiv(leftValue, rightValue),
              _ => throw new Exception($"\"{node.Text}\" is not a known operator.")
            }
          )
      );

    return builder;
  }


  public override LLVMBuilderRef Visit(FunctionCallExpression node) {
    var function = module.GetNamedFunction(node.Reference.Identifier.Name);

    if (function.Params.Length != node.Arguments.Count) {
      throw new Exception("Incorrect # arguments passed");
    }

    var argumentCount = (uint)node.Arguments.Count;
    var args          = new LLVMValueRef[Math.Max(argumentCount, 1)];

    for (var i = 0; i < argumentCount; i++) {
      args[i] = LLVMValueRef.CreateConstInt(
          LLVMTypeRef.Int32,
          (ulong)(node.Arguments[i].Value.AsT0 as NumericLiteral).Value
        );
    }

    ASTValueMap.Add(
        node,
        new StrongBox<LLVMValueRef>(
            builder.BuildCall(
                function,
                args,
                $"{node.Reference.Identifier.Name}-return"
              )
          )
      );

    return builder;
  }


  public override LLVMBuilderRef Visit(FunctionDeclaration node) {
    var function = module.GetNamedFunction(node.Identifier.Name);

    // Function was not previously defined.
    if (function.Handle == IntPtr.Zero) {
      // Set up the parameters.
      var paramCount = (uint)node.Parameters.Count;
      var @params    = new LLVMTypeRef[paramCount];

      // Loop over the params we defined in the function and assign them their types.
      for (var i = 0; i < paramCount; i++) {
        @params[i] = LLVMTypeRef.Int32;
      }

      // Create the function definition
      function = module.AddFunction(
          node.Identifier.Name,
          LLVMTypeRef.CreateFunction(LLVMTypeRef.Int32, @params)
        );
      function.Linkage = LLVMLinkage.LLVMExternalLinkage;

      // Loop over the params we defined in the function and assign them names.
      for (var i = 0; i < paramCount; i++) {
        var param = function.GetParam((uint)i);
        param.Name = node.Parameters[i].Identifier.Name;
      }
    }

    // Associate the LLVM function to the AST node.
    ASTValueMap.Add(node, new StrongBox<LLVMValueRef>(function));

    var functionBody = function.AppendBasicBlock("entry");

    // Set the "cursor" of the builder to the end of a new "basic block".
    builder.PositionAtEnd(functionBody);

    foreach (var statement in node.Body.AllStatements) {
      Visit(statement.Value as INode);
      builder.PositionAtEnd(functionBody);
    }

    builder.ClearInsertionPosition();

    // Validate the function we built:
    if (!function.VerifyFunction(LLVMVerifierFailureAction.LLVMReturnStatusAction)) {
      throw new LLVMResult(
          LLVMResultType.Error,
          () => { function.VerifyFunction(LLVMVerifierFailureAction.LLVMPrintMessageAction); },
          function.Dump
        );
    }

    return builder;
  }


  public override LLVMBuilderRef Visit(FunctionScope node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(FunctionScopeStatement node) {
    if (node.Value is Statement or Declaration) Visit(node.Value as INode);

    return builder;
  }


  public override LLVMBuilderRef Visit(LiteralExpression node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(Modifier node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(Module node) {
    Visit(node.TopLevel);
    return builder;
  }


  public override LLVMBuilderRef Visit(NamedTypeParameter node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(NumericLiteral node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(OperationalKeyword node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(Operator node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(PositionalParameter node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(ReferenceExpression node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(Statement node) {
    // Evaluate expressions
    switch (node.Expression) {
      case FunctionCallExpression expression:
        Visit(expression);
        break;
      case BinaryOperation operation:
        Visit(operation);
        break;
    }

    StrongBox<LLVMValueRef> llvmExpression = null;

    if (node.Expression is not null) {
      ASTValueMap.TryGetValue(node.Expression, out llvmExpression);
    }

    if (node.LeadingKeyword is not null) {
      ASTValueMap.Add(
          node,
          new StrongBox<LLVMValueRef>(
              node.LeadingKeyword.Type switch {
                OperationalKeywordType.Out => module.CallPrint(builder, llvmExpression.Value),
                OperationalKeywordType.Return => llvmExpression is null
                                                   ? builder.BuildRetVoid()
                                                   : builder.BuildRet(llvmExpression.Value)
              }
            )
        );
    }

    return builder;
  }


  public override LLVMBuilderRef Visit(TopLevel node) {
    // Define extern printf.
    var printf = module.DeclareFunction(
        "printf",
        LLVMTypeRef.Int32,
        Array.Empty<LLVMTypeRef>(),
        true
      );

    printf.FunctionCallConv = (uint)LLVMCallConv.LLVMCCallConv;
    printf.Linkage          = LLVMLinkage.LLVMExternalLinkage;

    var hello = module.DeclareFunction(
        "hello",
        LLVMTypeRef.Void,
        Array.Empty<LLVMTypeRef>()
      );

    // Define main func.
    var @params = new LLVMTypeRef[0];

    // Create the function definition
    var function = module.AddFunction(
        "main",
        LLVMTypeRef.CreateFunction(LLVMTypeRef.Int32, @params)
      );

    function.Linkage = LLVMLinkage.LLVMExternalLinkage;

    var functionBody = function.AppendBasicBlock("entry");

    // Set the "cursor" of the builder to the end of a new "basic block".
    builder.PositionAtEnd(functionBody);

    // Determine statements
    foreach (var statement in node.AllStatements) {
      Visit(statement.Value as INode);
      builder.PositionAtEnd(functionBody);
    }

    hello.Linkage = LLVMLinkage.LLVMExternalLinkage;

    builder.BuildCall(
        hello,
        Array.Empty<LLVMValueRef>()
      );

    //Visit(node.Body);
    // Create the return value.
    builder.BuildRet(LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 0));

    builder.ClearInsertionPosition();

    // Validate the function we built:
    if (!function.VerifyFunction(LLVMVerifierFailureAction.LLVMReturnStatusAction)) {
      throw new LLVMResult(
          LLVMResultType.Error,
          () => { function.VerifyFunction(LLVMVerifierFailureAction.LLVMPrintMessageAction); },
          function.Dump
        );
    }

    Compiler.MainFunction = function;

    return builder;
  }


  public override LLVMBuilderRef Visit(TypeIdentifier node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(TypeReference node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(Value node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(Void node) {
    throw new NotImplementedException();
  }
}
