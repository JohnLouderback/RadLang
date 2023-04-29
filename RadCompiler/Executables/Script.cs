using System.Runtime.InteropServices;
using LLVMSharp.Interop;
using RadCompiler.Utils;
using RadParser.AST.Node;
using static RadCompiler.Utils.GeneralUtils;

namespace RadCompiler;

public class Script : Executable, IExecutable {
  /// <summary>
  ///   This member is used to house the delegate needed to make the pointer to the main
  ///   function of the compiled program
  /// </summary>
  public delegate int ProgramEntrypointPointer();

  /// <summary>
  ///   A reference to the top-level function of the module. This is used to make the pointer to the
  ///   main function so that the JIT can find it and we can run the program as a script rather than
  ///   compiling it to an executable.
  /// </summary>
  public LLVMValueRef MainFunction { get; set; }


  public override LLVMValueRef Build(Module ast) {
    MainFunction = base.Build(ast);
    return MainFunction;
  }


  public override void Run() {
    UpdateRunStatus("Initializing run.");
    // Ensure that the root module is not null. If it is, then we cannot run the program because it
    // was never built.
    if (RootModule is null) {
      throw new LLVMResult(
          LLVMResultType.Error,
          () => Console.WriteLine(
              "Root module is null for this script. `Run` was likely called before `Build`. Call `Build` prior to calling `Run`."
            )
        );
    }

    // Ensure that the execution engien is not null. If it is, then we cannot run the program
    // because it was never built.
    if (RootModule is null) {
      throw new LLVMResult(
          LLVMResultType.Error,
          () => Console.WriteLine(
              "Execution engine is null for this script. `Run` was likely called before `Build`. Call `Build` prior to calling `Run`."
            )
        );
    }

    UpdateRunStatus("Running script.");
    // Create a pointer to the main function so that we can run the program as a script.
    var mainFunctionPointer = (ProgramEntrypointPointer)Marshal.GetDelegateForFunctionPointer(
        ((LLVMExecutionEngineRef)ExecutionEngine).GetPointerToGlobal(MainFunction),
        typeof(ProgramEntrypointPointer)
      );

    // Call the main function and store the exit code. If all goes well, it should be 0.
    var exitCode = mainFunctionPointer();

    UpdateRunStatus($"Script finished executing with exit code {exitCode}.");
  }


  protected override void Initialize() {
    // Load the runtime helper library and check to see if there was any error in doing so.
    // If an error occurred, throw an exception as a message indicating that the library could not
    // be loaded.
    var dynRuntimeLib = GetPlatformSpecificDynamicRuntimeLib();
    if (LLVMUtils.LoadLibraryPermanently(Path.Combine(GetApplicationDirectory(), dynRuntimeLib))) {
      throw new LLVMResult(
          LLVMResultType.Error,
          () => Console.WriteLine(
              $"Error loading runtime helper library \"{Path.Combine(GetApplicationDirectory(), dynRuntimeLib)}\"."
            )
        );
    }

    // Initialize the compilation targets.
    LLVM.LinkInMCJIT();
    LLVM.InitializeX86TargetMC();
    LLVM.InitializeX86Target();
    LLVM.InitializeX86TargetInfo();
    LLVM.InitializeX86AsmParser();
    LLVM.InitializeX86AsmPrinter();
  }
}
