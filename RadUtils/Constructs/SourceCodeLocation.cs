namespace RadUtils.Constructs;

public record struct SourceCodeLocation {
  public int Line { get; init; }
  public int EndLine { get; init; }
  public int Column { get; init; }
  public int EndColumn { get; init; }
}
