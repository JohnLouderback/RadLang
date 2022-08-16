using System.Runtime.InteropServices;
using LLVMSharp.Interop;
using RadCompiler.Utils;
using RadParser.AST.Node;

namespace RadCompiler;

public class Compiler {
  public delegate double D();

  public static LLVMValueRef MainFunction { get; set; }


  public void Compile(Module ast) {
    // Creates the application. A module is the whole program, not just a single file. 
    var module = LLVMModuleRef.CreateWithName("Rad");

    // Creates a context, such as for a thread.
    var context = LLVMContextRef.Create();

    // Creates a "builder" for generating IR code.
    var builder = LLVMBuilderRef.Create(context);

    LLVM.LinkInMCJIT();
    LLVM.InitializeX86TargetMC();
    LLVM.InitializeX86Target();
    LLVM.InitializeX86TargetInfo();
    LLVM.InitializeX86AsmParser();
    LLVM.InitializeX86AsmPrinter();

    if (!module.TryCreateExecutionEngine(out var engine, out var msg)) {
      throw new LLVMResult(LLVMResultType.Error, () => Console.WriteLine(msg));
    }

    var passManager = module.CreateFunctionPassManager();

    var codeGenerator = new CodeGenASTVisitor(module, builder, context);

    codeGenerator.Visit(ast);

    if (!module.TryVerify(LLVMVerifierFailureAction.LLVMReturnStatusAction, out var str)) {
      throw new LLVMResult(LLVMResultType.Error, () => Console.WriteLine(str), module.Dump);
    }

    Console.WriteLine();
    throw new LLVMResult(
        LLVMResultType.Success,
        () => {
          var dFunc = (D)Marshal.GetDelegateForFunctionPointer(
              engine.GetPointerToGlobal(MainFunction),
              typeof(D)
            );

          // passManager.Run(MainFunction);

          //            LLVM.DumpValue(anonymousFunction); // Dump the function for exposition purposes.
          Console.WriteLine("Evaluated to " + dFunc());
        }
      );

    //throw new LLVMResult(LLVMResultType.Success, () => { module.Dump(); });
    var test = "''";
  }
}
