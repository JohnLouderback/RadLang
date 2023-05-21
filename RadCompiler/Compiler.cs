using RadCompiler.Utils;
using RadParser.AST.Node;

namespace RadCompiler;

/// <summary>
///   The <c> Compiler </c> class is responsible for compiling the AST into LLVM IR code. Additionally,
///   it can perform other compilation-related tasks, such as running the LLVM toolchain to generate
///   an executable, running a source file as a script, or generating object code.
/// </summary>
public class Compiler {
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
  /// <returns>
  ///   An <see cref="LLVMResult"> LLVMResult </see> object containing the result of the compilation.
  /// </returns>
  /// <exception cref="LLVMResult">
  ///   LLVM Result when the compilation fails due to underlying LLVM errors. This indicates an
  ///   issue with the compiler itself rather than the code being compiled.
  /// </exception>
  public LLVMResult Compile(Module ast) {
    var script = new Script();

    void StatusListener(object sender, CompilerEventArgs args) =>
      UpdateStatus(args.Message, args.Details);

    // Bind the build status listener and forward the event to any subscribers.
    script.BuildStatus += StatusListener;

    script.Build(ast);

    // Remove the build status listener.
    script.BuildStatus -= StatusListener;

    // If the compilation was successful, run the code and use the code's output as the message to
    // log to the console.
    return new LLVMResult(
        LLVMResultType.Success,
        () => {
          // Bind the run status listener and forward the event to any subscribers.
          script.RunStatus += StatusListener;

          // Execute the built code as a script.
          script.Run();

          // Remove the run status listener.
          script.RunStatus -= StatusListener;

          // engine.TargetMachine.TryEmitToFile(
          //     module,
          //     Path.Combine(Directory.GetCurrentDirectory(), "rad.o"),
          //     LLVMCodeGenFileType.LLVMObjectFile,
          //     out var str
          //   );
        }
      );
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
