using Antlr4.Runtime;

namespace RadParser.AST.Node; 

public abstract class Literal : Node<INode> {
  public Literal(ParserRuleContext context) : base(context) {
  }
}
