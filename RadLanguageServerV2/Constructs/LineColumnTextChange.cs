namespace RadLanguageServerV2.Constructs;

/// <summary>
///   Represents a change to a document in terms of line and column numbers. This is used to represent
///   changes to a document in terms of the line and column numbers of the start and end of the change.
/// </summary>
public class LineColumnTextChange {
  public string NewText { get; set; }
  public int StartLine { get; set; }
  public int StartColumn { get; set; }
  public int EndLine { get; set; }
  public int EndColumn { get; set; }
}
