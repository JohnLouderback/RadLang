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
