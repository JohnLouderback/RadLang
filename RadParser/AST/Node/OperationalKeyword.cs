using Antlr4.Runtime;

namespace RadParser.AST.Node; 

public enum OperationalKeywordType {
  Return,
  Out
}

public class OperationalKeyword : Keyword {
  public OperationalKeywordType Type { get; internal set; }


  public OperationalKeyword(ParserRuleContext context) : base(context) {
  }
}
