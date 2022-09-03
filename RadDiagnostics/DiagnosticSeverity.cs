namespace RadDiagnostics;

/// <summary>
///   An enum representing the severity levels of a diagnostic.
/// </summary>
public enum DiagnosticSeverity {
  /// <summary>
  ///   The diagnostic represents a definite error. This likely means the program will not compile or
  ///   will most definitely fail at runtime.
  /// </summary>
  Error,

  /// <summary>
  ///   The diagnostic represents a warning that something is likely erroneous and may result in
  ///   unexpected behavior at runtime. Alternatively, this could represent an anti-pattern or likely
  ///   conceptual issue.
  /// </summary>
  Warning,

  /// <summary>
  ///   The diagnostic represents information about a piece of code.
  /// </summary>
  Information,

  /// <summary>
  ///   The diagnostic represents a hint. As an example, this could be a suggestion for a better or more
  ///   readable way of writing the code in question.
  /// </summary>
  Hint
}
