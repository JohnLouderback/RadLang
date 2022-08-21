using System.Diagnostics;
using System.Runtime.InteropServices;
using LLVMSharp.Interop;
using RadCompiler.Utils;
using RadParser.AST.Node;
using static RadCompiler.Utils.GeneralUtils;

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

  /// <summary>
  ///   This member is used to house the delegate needed to make the pointer to the main
  ///   function.
  /// </summary>
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
    // Load the runtime helper library and check to see if there was any error in doing so.
    if (LLVMUtils.LoadLibraryPermanently(Path.Combine(GetApplicationDirectory(), "RadLib.dll"))) {
      throw new LLVMResult(
          LLVMResultType.Error,
          () => Console.WriteLine(
              $"Error loading runtime helper library \"{Path.Combine(GetApplicationDirectory(), "RadLib.dll")}\"."
            )
        );
    }

    // Creates the application. A module is the whole program, not just a single file.
    var module = LLVMModuleRef.CreateWithName("Rad");

    // Creates a context, such as for a thread.
    var context = LLVMContextRef.Create();

    // Creates a "builder" for generating IR code.
    var builder = LLVMBuilderRef.Create(context);

    // Initialize the compilation targets.
    LLVM.LinkInMCJIT();
    LLVM.InitializeX86TargetMC();
    LLVM.InitializeX86Target();
    LLVM.InitializeX86TargetInfo();
    LLVM.InitializeX86AsmParser();
    LLVM.InitializeX86AsmPrinter();

    // Create the execution engine for the application.
    if (!module.TryCreateExecutionEngine(out var engine, out var msg)) {
      throw new LLVMResult(LLVMResultType.Error, () => Console.WriteLine(msg));
    }

    // Create the optimization pass manager for the application.
    var passManager = module.CreateFunctionPassManager();

    // Instantiate the code generator.
    var codeGenerator = new CodeGenASTVisitor(module, builder, context);

    UpdateStatus("Starting code compilation.", DateTime.Now.ToLongTimeString());

    // Start a timer to measure how longer code generation took.
    var timer = Stopwatch.StartNew();

    // Generate the code by visiting each node in the AST and deciding what IR should be generated.
    codeGenerator.Visit(ast);

    UpdateStatus("Code compilation finished.", $"Took {GenerateTimeSpanString(timer)}");

    // Check for any issues that occurred after generating the code. If there were any, throw.
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
  }

  /**
   * Test
   */
  private static string GenerateTimeSpanString(Stopwatch timer) {
    string totalTimeString;
    totalTimeString = timer.Elapsed.TotalMilliseconds > 1000
                        ? $"{timer.Elapsed.TotalSeconds.ToString()} seconds"
                        : $"{timer.Elapsed.TotalMilliseconds.ToString()} ms";

    return totalTimeString;
  }


  /// <summary>
  ///   A convenience method for invoking the `Status` event. Sends a status of update of
  ///   where the compiler is at, to any listeners.
  /// </summary>
  /// <param name="msg"> The status update message. </param>
  /// <param name="details"> Any additional information to be included. </param>
  private void UpdateStatus(string msg, string? details = null) {
    Status?.Invoke(
        this,
        new CompilerEventArgs(msg, details)
      );
  }
}
