using Antlr4.Runtime;
using RadDiagnostics;
using RadUtils;
using RadUtils.Constructs;

namespace RadLexer;

public readonly struct SyntaxError : IDiagnostic {
  public readonly IRecognizer Recognizer;
  public readonly IToken OffendingSymbol;
  public readonly int Line;
  public readonly int CharPositionInLine;
  public readonly RecognitionException Exception;


  public SyntaxError(
    IRecognizer recognizer,
    IToken offendingSymbol,
    int line,
    int charPositionInLine,
    string message,
    RecognitionException exception
  ) {
    Recognizer         = recognizer;
    OffendingSymbol    = offendingSymbol;
    Line               = line;
    CharPositionInLine = charPositionInLine;
    Message            = $"{nameof(SyntaxError).PascalCaseToTitleCase()}: {message}";
    Exception          = exception;
  }


  /// <inheritdoc />
  public DiagnosticSeverity Severity { get; } = DiagnosticSeverity.Error;

  public SourceCodeLocation Location => new() {
    Line      = Line,
    Column    = CharPositionInLine,
    EndLine   = Line,
    EndColumn = CharPositionInLine + OffendingSymbol.Text.Length
  };

  /// <summary>
  ///   The message explaining the cause of the error.
  /// </summary>
  public string Message { get; }
}
