using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node;

public abstract class Keyword : Node<INode> {
  public Keyword(ParserRuleContext context) : base(context) {}


  public Keyword(ITerminalNode tokenNode) :
    base(tokenNode.Parent.RuleContext as ParserRuleContext) {
    var token = tokenNode.Payload as IToken;
    Text   = tokenNode.GetText();
    Line   = token.Line;
    Column = token.Column;
    Width  = Text.Length;
  }
}
