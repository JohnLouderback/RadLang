using RadUtils.Constructs;

namespace RadDiagnostics;

public interface IDiagnostic {
  /// <summary>
  ///   Represents the severity level of the diagnostic.
  /// </summary>
  DiagnosticSeverity Severity { get; }

  /// <summary>
  ///   The location in the source code where the diagnostic occurred.
  /// </summary>
  SourceCodeLocation Location { get; }

  /// <summary>
  ///   The message explaining the diagnosis.
  /// </summary>
  string Message { get; }
}
