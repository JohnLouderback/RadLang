using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node;

public enum OperatorType {
  Star,
  ForwardSlash,
  Plus,
  Minus
}

public class Operator : Node<INode> {
  public OperatorType Type { get; internal set; }

  public Operator(ParserRuleContext context) : base(context) {}


  public Operator(ITerminalNode tokenNode) :
    base(tokenNode.Parent.RuleContext as ParserRuleContext) {
    var token = tokenNode.Payload as IToken;
    Text   = tokenNode.GetText();
    Line   = token.Line;
    Column = token.Column;
    Width  = Text.Length;
  }
}
