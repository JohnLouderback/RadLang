using Antlr4.Runtime;

namespace RadLexer;

public class SyntaxErrorListener : BaseErrorListener {
  public readonly List<SyntaxError> Errors = new();


  public override void SyntaxError(
    TextWriter output,
    IRecognizer recognizer,
    IToken offendingSymbol,
    int line,
    int charPositionInLine,
    string msg,
    RecognitionException e
  ) {
    Errors.Add(new SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e));
  }
}
