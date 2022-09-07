using RadDiagnostics;

namespace RadTypeChecker.TypeErrors;

public interface ITypeError : IDiagnostic {
  /// <summary>
  ///   A unique numeric identifier for the error.
  /// </summary>
  uint ErrorCode { get; }

  /// <summary>
  ///   A string representation of the numeric error code with any additional identifiers added.
  /// </summary>
  string ErrorCodeString { get; }
}
