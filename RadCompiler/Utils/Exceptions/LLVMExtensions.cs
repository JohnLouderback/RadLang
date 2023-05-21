using LLVMSharp.Interop;
using RadParser.AST.Node;

namespace RadCompiler.Utils;

public static class LLVMExtensions {
  public static LLVMValueRef CallPrint(
    this LLVMModuleRef module,
    LLVMBuilderRef builder,
    LLVMValueRef value
  ) {
    var          printf      = module.GetNamedFunction("printf");
    var          printIntStr = builder.BuildGlobalStringPtr("%d\n", "str");
    LLVMValueRef str;

    if (value.TypeOf == LLVMTypeRef.Int32) {
      str = printIntStr;
    }
    else {
      throw new ArgumentException(
          $"Could not determine the typeof of the \"{nameof(value)}\" argument to \"printf\"."
        );
    }

    return builder.BuildCall(
        printf,
        new[] {
          // It is necessary to cast the string to a pointer to an array of i8 (char). This is
          // because LLVM does not have a string type, so we must use a pointer to an array of
          // i8 instead. Without this cast, the compiler will throw an exception because the type
          // would be incorrect for the call to "printf".
          LLVMValueRef.CreateConstBitCast(str, LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0)),
          value
        }
      );
  }


  public static LLVMValueRef DeclareFunction(
    this LLVMModuleRef module,
    string functionName,
    LLVMTypeRef returnType,
    IEnumerable<LLVMTypeRef> paramTypes,
    bool hasVariadicArgs = false
  ) {
    return module.DeclareFunction(functionName, returnType, paramTypes.ToArray(), hasVariadicArgs);
  }


  public static LLVMValueRef DeclareFunction(
    this LLVMModuleRef module,
    string functionName,
    LLVMTypeRef returnType,
    LLVMTypeRef[] paramTypes,
    bool hasVariadicArgs = false
  ) {
    return module.AddFunction(
        functionName,
        LLVMTypeRef.CreateFunction(
            returnType,
            paramTypes,
            hasVariadicArgs
          )
      );
  }


  public static LLVMValueRef DefineFunction(
    this LLVMModuleRef module,
    LLVMBuilderRef builder,
    string functionName,
    LLVMTypeRef returnType,
    IEnumerable<LLVMTypeRef> paramTypes,
    IEnumerable<FunctionScopeStatement> statements,
    Action<FunctionScopeStatement> statementHandler,
    bool hasVariadicArgs = false
  ) {
    return module.DefineFunction(
        builder,
        functionName,
        returnType,
        paramTypes.ToArray(),
        statements,
        statementHandler,
        hasVariadicArgs
      );
  }


  public static LLVMValueRef DefineFunction(
    this LLVMModuleRef module,
    LLVMBuilderRef builder,
    string functionName,
    LLVMTypeRef returnType,
    LLVMTypeRef[] paramTypes,
    IEnumerable<FunctionScopeStatement> statements,
    Action<FunctionScopeStatement> statementHandler,
    bool hasVariadicArgs = false
  ) {
    var function = module.DeclareFunction(functionName, returnType, paramTypes, hasVariadicArgs);
    var functionBody = function.AppendBasicBlock("entry");

    // Set the "cursor" of the builder to the end of a new "basic block".
    builder.PositionAtEnd(functionBody);

    // Determine statements
    foreach (var statement in statements) {
      // Call the statement handler provided.
      statementHandler(statement);

      // Reposition "cursor" of the builder, in case the handler moved it.
      builder.PositionAtEnd(functionBody);
    }

    builder.ClearInsertionPosition();

    return function;
  }
}
