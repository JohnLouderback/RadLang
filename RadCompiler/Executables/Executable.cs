using System.Diagnostics;
using LLVMSharp.Interop;
using RadCompiler.CodeGen;
using RadCompiler.Utils;
using RadParser.AST.Node;
using RadProject;
using static RadCompiler.Utils.GeneralUtils;

namespace RadCompiler;

/// <inheritdoc />
/// <summary>
///   The <c> Executable </c> class is the base class for all executable constructs in the Rad.
///   For instance a compiled program or a script. It represents a build-able and runnable unit. It
///   provides a default implementation for <see cref="T:RadCompiler.IExecutable" />.
/// </summary>
public abstract class Executable : IExecutable {
  protected LLVMModuleRef? RootModule { get; set; }
  protected LLVMExecutionEngineRef? ExecutionEngine { get; set; }

  public event CompilerEventHandler? BuildStatus;
  public event CompilerEventHandler? RunStatus;


  public virtual LLVMValueRef Build(Module ast) {
    // Perform any necessary initialization in order to build the executable.
    Initialize();

    // Creates the application. A module is the whole program, not just a single file.
    var module = LLVMModuleRef.CreateWithName("Rad");

    // Create the execution engine for the application.
    if (!module.TryCreateExecutionEngine(out var engine, out var msg)) {
      throw new LLVMResult(LLVMResultType.Error, () => Console.WriteLine(msg));
    }

    // Store the execution engine on the class for access from other methods and derived classes.
    ExecutionEngine = engine;

    // Store the module on the class for access from other methods and derived classes.
    RootModule = module;

    // Create the optimization pass manager for the executable.
    var passManager = module.CreateFunctionPassManager();

    // Creates a context, such as for a thread.
    var context = LLVMContextRef.Create();

    // Creates a "builder" for generating IR code.
    var builder = LLVMBuilderRef.Create(context);

    var codeGenerator = new CodeGenASTVisitor(module, builder, context);

    // Start a timer to measure how longer code generation took.
    var timer = new Stopwatch();

    UpdateBuildStatus("Starting code compilation.", DateTime.Now.ToLongTimeString());

    // Start the timer.
    timer.Start();

    // Generate the code by visiting each node in the AST and deciding what IR should be generated.
    codeGenerator.Visit(ast);

    // End measuring the time taken to generate the code.
    timer.Stop();

    UpdateBuildStatus("Code compilation finished.", $"Took {GenerateTimeSpanString(timer)}");

    // Check for any issues that occurred after generating the code. If there were any, throw.
    if (!module.TryVerify(LLVMVerifierFailureAction.LLVMReturnStatusAction, out var str)) {
      throw new LLVMResult(LLVMResultType.Error, () => Console.WriteLine(str), module.Dump);
    }

    return codeGenerator.MainFunction;
  }


  public virtual LLVMValueRef Build(Project project) {
    return Build(project.EntryPoint);
  }


  public abstract void Run();

  protected abstract void Initialize();


  /// <summary>
  ///   A convenience method for invoking the <see cref="Executable.BuildStatus"> BuildStatus </see>
  ///   event. Sends a status of update of where the progress of build method is at, to any
  ///   listeners.
  /// </summary>
  /// <param name="msg"> The status update message. </param>
  /// <param name="details"> Any additional information to be included. </param>
  protected void UpdateBuildStatus(string msg, string? details = null) {
    BuildStatus?.Invoke(
        this,
        new CompilerEventArgs(msg, details)
      );
  }


  /// <summary>
  ///   A convenience method for invoking the <see cref="Executable.RunStatus"> RunStatus </see>
  ///   event. Sends a status of update of where the progress of the run method is at, to any
  ///   listeners.
  /// </summary>
  /// <param name="msg"> The status update message. </param>
  /// <param name="details"> Any additional information to be included. </param>
  protected void UpdateRunStatus(string msg, string? details = null) {
    RunStatus?.Invoke(
        this,
        new CompilerEventArgs(msg, details)
      );
  }
}
