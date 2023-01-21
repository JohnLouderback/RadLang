namespace RadUtils.Constructs;

/// <inheritdoc />
/// <summary>
///   Represents a location in the source code.
/// </summary>
public record struct SourceCodeLocation {
  /// <summary>
  ///   The 1-based line index where this location begins.
  /// </summary>
  public int Line { get; init; }

  /// <summary>
  ///   The 1-based line index where this location ends.
  /// </summary>
  public int EndLine { get; init; }

  /// <summary>
  ///   The 1-based column index where this location begins.
  /// </summary>
  public int Column { get; init; }

  /// <summary>
  ///   The 1-based column index where this location ends.
  /// </summary>
  public int EndColumn { get; init; }
}
