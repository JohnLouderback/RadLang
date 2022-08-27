using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using RadParser.Utils.Attributes;

namespace RadParser.AST.Node; 

public class Reference : Node<INode>, IReference<Identifier> {
  public virtual Identifier Identifier { get; set; }

  public Reference(ITerminalNode tokenNode) : base(tokenNode.Parent.RuleContext as ParserRuleContext) {
    var token = tokenNode.Payload as IToken;
    Text = tokenNode.GetText();
    Line = token.Line;
    Column = token.Column;
    Width = Text.Length;
  }
}
