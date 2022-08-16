using LLVMSharp.Interop;
using RadCompiler.Utils;
using RadParser;
using RadParser.AST.Node;
using Void = RadParser.AST.Node.Void;

namespace RadCompiler;

public class CodeGenASTVisitor : BaseASTVisitor<LLVMBuilderRef> {
  private readonly LLVMModuleRef module;

  private readonly LLVMBuilderRef builder;

  private readonly LLVMContextRef context;

  private readonly Dictionary<string, LLVMValueRef> namedValues = new();


  public CodeGenASTVisitor(LLVMModuleRef module, LLVMBuilderRef builder, LLVMContextRef context) {
    this.module  = module;
    this.builder = builder;
    this.context = context;
  }


  public override LLVMBuilderRef Visit(BinaryOperation node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(FunctionCallExpression node) {
    var function = module.GetNamedFunction(node.Reference.Identifier.Name);

    if (function.Params.Length != node.Arguments.Count) {
      throw new Exception("Incorrect # arguments passed");
    }

    var argumentCount = (uint)node.Arguments.Count;
    var args          = new LLVMValueRef[Math.Max(argumentCount, val2: 1)];

    for (var i = 0; i < argumentCount; i++) {
      args[i] = LLVMValueRef.CreateConstInt(
          LLVMTypeRef.Int32,
          (ulong)(node.Arguments[i].Value.AsT0 as NumericLiteral).Value
        );
    }

    var retVal = builder.BuildCall(function, args, Name: "calltmp");
    var printf = module.GetNamedFunction("printf");
    var str    = builder.BuildGlobalStringPtr(Str: "Hello%i\n", Name: "str");

    // var str2   = context.GetConstString("%i", false);

    // var args = new[] { str };
    builder.BuildCall(
        printf,
        new[] {
          str,
          LLVMValueRef.CreateConstInt(
              LLVMTypeRef.Int32,
              N: 10
            )
        },
        Name: ""
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

    var functionBody = function.AppendBasicBlock("entry");

    // Set the "cursor" of the builder to the end of a new "basic block".
    builder.PositionAtEnd(functionBody);

    //Visit(node.Body);
    // Create the return value.
    builder.BuildRet(LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, N: 10));

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
    if (node.Expression is FunctionCallExpression) Visit(node.Expression as FunctionCallExpression);

    return builder;
  }


  public override LLVMBuilderRef Visit(TopLevel node) {
    // Define extern printf.
    var printf = module.AddFunction(
        Name: "printf",
        LLVMTypeRef.CreateFunction(
            LLVMTypeRef.Int32,

            // Specifying an empty array because variadic args don't require the typing.
            Array.Empty<LLVMTypeRef>(),
            IsVarArg: true
          )
      );

    //printf.FunctionCallConv = (uint)LLVMCallConv.LLVMCCallConv;
    //printf.Linkage          = LLVMLinkage.LLVMExternalLinkage;

    // Define main func.
    var @params = new LLVMTypeRef[0];

    // Create the function definition
    var function = module.AddFunction(
        Name: "main",
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

    //Visit(node.Body);
    // Create the return value.
    builder.BuildRet(LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, N: 0));

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
