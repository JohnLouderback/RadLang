using Antlr4.Runtime;

namespace RadParser.AST.Node; 

public class Statement : Node<INode> {
  public OperationalKeyword? LeadingKeyword { get; internal set; }
  public Expression? Expression { get; internal set; }


  public Statement(ParserRuleContext context) : base(context) {
  }
}
