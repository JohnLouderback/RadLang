using System.Diagnostics;
using System.Runtime.InteropServices;
using LLVMSharp.Interop;
using RadCompiler.Utils;
using RadParser.AST.Node;

namespace RadCompiler;

public class CompilerEventArgs {
  public string Message { get; } // readonly
  public string? Details { get; } // readonly


  public CompilerEventArgs(string msg, string? details = null) {
    Message = msg;
    Details = details;
  }
}

public class Compiler {
  public delegate void CompilerEventHandler(object sender, CompilerEventArgs e);

  public delegate double D();

  /// <summary>
  ///   A reference to the top-level function of the module.
  /// </summary>
  public static LLVMValueRef MainFunction { get; set; }

  /// <summary>
  ///   The `Status` event is used to send status updates to subscribers.
  /// </summary>
  public event CompilerEventHandler Status;


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

    UpdateStatus("Starting code compilation.", DateTime.Now.ToLongTimeString());
    var timer = Stopwatch.StartNew();

    // Generate the code.
    codeGenerator.Visit(ast);

    UpdateStatus("Code compilation finished.", $"Took {GenerateTimeSpanString(timer)}");

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

          //passManager.Run(module);
          timer.Restart();
          UpdateStatus("Code execution started.", DateTime.Now.ToLongTimeString());
          var exitCode = dFunc();
          UpdateStatus("Code execution finished.", $"Took {GenerateTimeSpanString(timer)}");
          Console.WriteLine($"Process finished with exit code {exitCode}.");

          engine.TargetMachine.TryEmitToFile(
              module,
              Path.Combine(Directory.GetCurrentDirectory(), "rad.o"),
              LLVMCodeGenFileType.LLVMObjectFile,
              out var str
            );
        }
      );

    //throw new LLVMResult(LLVMResultType.Success, () => { module.Dump(); });
    var test = "''";
  }


  public void UpdateStatus(string msg, string? details = null) {
    Status?.Invoke(
        this,
        new CompilerEventArgs(msg, details)
      );
  }


  private static string GenerateTimeSpanString(Stopwatch timer) {
    string totalTimeString;
    if (timer.Elapsed.TotalMilliseconds > 1000) {
      totalTimeString = $"{timer.Elapsed.TotalSeconds.ToString()} seconds";
    }
    else {
      totalTimeString = $"{timer.Elapsed.TotalMilliseconds.ToString()} ms";
    }

    return totalTimeString;
  }
}
