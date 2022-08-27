using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node;

public enum DeclaratorKeywordType {
  Function
}

public class DeclaratorKeyword : Keyword {
  public DeclaratorKeywordType Type { get; internal set; }
  public DeclaratorKeyword(ParserRuleContext context) : base(context) {}

  public DeclaratorKeyword(ITerminalNode token) : base(token) {}
}
