using Antlr4.Runtime;

namespace RadParser.AST.Node; 

public class Keyword : Node<INode> {
  public Keyword(ParserRuleContext context) : base(context) {
  }
}
