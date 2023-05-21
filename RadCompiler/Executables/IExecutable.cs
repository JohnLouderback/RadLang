using LLVMSharp.Interop;
using RadParser.AST.Node;
using RadProject;

namespace RadCompiler;

/// <summary>
///   The <c> IExecutable </c> interface is the base interface for all executable constructs in
///   Rad. For instance a compiled program or a script. It represents a build-able and runnable unit.
/// </summary>
public interface IExecutable {
  /// <summary>
  ///   The <c> BuildStatus </c> event is used to send status updates to subscribers as to the
  ///   current status of the build process.
  /// </summary>
  public event CompilerEventHandler? BuildStatus;

  /// <summary>
  ///   The <c> RunStatus </c> event is used to send status updates to subscribers as to the
  ///   current status of the run process.
  /// </summary>
  public event CompilerEventHandler? RunStatus;


  /// <summary>
  ///   Builds the "executable" from the AST.
  /// </summary>
  /// <param name="ast">
  ///   The AST to build the executable from. This should represent the "root" of the AST. For
  ///   instance, the root of the entry point of the program.
  /// </param>
  /// <returns>
  ///   The <see cref="LLVMValueRef" /> of the entry point of the program – the main function.
  /// </returns>
  LLVMValueRef Build(Module ast);


  /// <summary>
  ///   Builds the "executable" from a Rad project.
  /// </summary>
  /// <param name="project">
  ///   The Rad project containing the entry-point source file to build the executable from.
  /// </param>
  /// <returns>
  ///   The <see cref="LLVMValueRef" /> of the entry point of the program – the main function.
  /// </returns>
  LLVMValueRef Build(Project project);


  /// <summary>
  ///   Runs, or executes, the executable. This should be called after <c> Build </c> has been
  ///   called. It is optional, of course, to run an executable after building it, but it is
  ///   necessary to build it prior to running it.
  /// </summary>
  void Run();
}
