using System.Diagnostics;
using System.Runtime.InteropServices;
using LLVMSharp.Interop;
using RadCompiler.CodeGen;
using RadCompiler.Utils;
using RadParser.AST.Node;
using static RadCompiler.Utils.GeneralUtils;

namespace RadCompiler;

/// <summary>
///   Provides details regarding the status of the compiler at any given time.
/// </summary>
public class CompilerEventArgs {
  /// <summary>
  ///   The message signifying the status of the compiler at this time.
  /// </summary>
  public string Message { get; } // readonly

  /// <summary>
  ///   Additional details regarding the status of the compiler at this time.
  /// </summary>
  public string? Details { get; } // readonly


  /// <inheritdoc cref="CompilerEventArgs" />
  /// <param name="msg">
  ///   The message signifying the status of the compiler at this time.
  /// </param>
  /// <param name="details">
  ///   Additional details regarding the status of the compiler at this time.
  /// </param>
  public CompilerEventArgs(string msg, string? details = null) {
    Message = msg;
    Details = details;
  }
}

/// <summary>
///   The <c> Compiler </c> class is responsible for compiling the AST into LLVM IR code. Additionally,
///   it can perform other compilation-related tasks, such as running the LLVM toolchain to generate
///   an executable, running a source file as a script, or generating object code.
/// </summary>
public class Compiler {
  /// <summary>
  ///   The <c> CompilerEventHandler </c> delegate is used to handle the <c> Status </c> event.
  /// </summary>
  public delegate void CompilerEventHandler(object sender, CompilerEventArgs e);

  /// <summary>
  ///   This member is used to house the delegate needed to make the pointer to the main
  ///   function.
  /// </summary>
  public delegate int D();

  /// <summary>
  ///   A reference to the top-level function of the module. This is used to make the pointer to the
  ///   main function so that the JIT can find it and we can run the program as a script rather than
  ///   compiling it to an executable.
  /// </summary>
  public static LLVMValueRef MainFunction { get; set; }

  /// <summary>
  ///   The <c> Status </c> event is used to send status updates to subscribers.
  /// </summary>
  public event CompilerEventHandler? Status;


  /// <summary>
  ///   The <c> Compile </c> method is used to compile the AST into LLVM IR code and then generate
  ///   an executable.
  /// </summary>
  /// <param name="ast">
  ///   The abstract syntax tree to use as the entry point for code generation.
  /// </param>
  /// <exception cref="LLVMResult">
  ///   Rather than return a value, this method will always throw an exception. The exception will
  ///   contain the result of the compilation process, which can be used to determine whether or not
  ///   the compilation was successful. This is done because LLVM outputs results to stdout and we
  ///   don't want to clutter the console with LLVM's output and instead want to handle it ourselves.
  ///   To this end, the exception is used as control flow mechanism to return to the caller that
  ///   should handle the result.
  /// </exception>
  public void Compile(Module ast) {
    // Load the runtime helper library and check to see if there was any error in doing so.
    // If an error occurred, throw an exception as a message indicating that the library could not
    // be loaded.
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
    // If the compilation was successful, run the code and use the code's output as the message to
    // log to the console.
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


  /// <summary>
  ///   Takes a timer that can be best expressed in either seconds or milliseconds and outputs a string
  ///   using the best candidate. If the timer is greater than 1 second, the string is output in seconds rather than
  ///   milliseconds.
  /// </summary>
  private static string GenerateTimeSpanString(Stopwatch timer) {
    string totalTimeString;
    totalTimeString = timer.Elapsed.TotalMilliseconds > 1000
                        ? $"{timer.Elapsed.TotalSeconds.ToString()} seconds"
                        : $"{timer.Elapsed.TotalMilliseconds.ToString()} ms";

    return totalTimeString;
  }


  /// <summary>
  ///   A convenience method for invoking the <see cref="Compiler.Status"> Status </see> event. Sends a status of update of
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
