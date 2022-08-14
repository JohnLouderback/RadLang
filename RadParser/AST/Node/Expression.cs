using Antlr4.Runtime;

namespace RadParser.AST.Node; 

public abstract class Expression : Node<INode> {
  public Expression(ParserRuleContext context) : base(context) {
  }
}
