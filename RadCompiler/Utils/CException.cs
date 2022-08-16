namespace RadCompiler.Utils;

/// <summary>
///   "C Exception" is an exception to throw when an error occurs from C library. The `GetMessage`
///   property exists as a way to get a message that might output to the stdout normally.
/// </summary>
public abstract class CException : Exception {
  public Action GetMessage { get; set; }


  public CException(Action getMessage) {
    GetMessage = getMessage;
  }
}
