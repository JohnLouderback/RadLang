namespace RadUtils.Constructs;

/// <summary>
///   Represents the location of a conceptual "cursor" within the document.
/// </summary>
public struct Cursor {
  /// <summary>
  ///   The 1-based line number where the cursor is placed.
  /// </summary>
  public uint Line;

  /// <summary>
  ///   The 1-based column number where the cursor is placed.
  /// </summary>
  public uint Column;
}
