using Antlr4.Runtime;

namespace RadParser.AST.Node; 

public class Expression : Node<INode> {
  public Expression(ParserRuleContext context) : base(context) {
  }
}
