using Antlr4.Runtime;

namespace RadLexer;

public class SyntaxErrorStrategy : DefaultErrorStrategy {
  /// <inheritdoc />
  protected override void ReportUnwantedToken(Parser recognizer) {
    if (InErrorRecoveryMode(recognizer)) {
      return;
    }

    BeginErrorCondition(recognizer);
    var currentToken = recognizer.CurrentToken;
    var msg =
      $"Unexpected extraneous token \"{GetTokenErrorDisplay(currentToken)}\". Try removing this token.";
    recognizer.NotifyErrorListeners(currentToken, msg, null);
  }
}
