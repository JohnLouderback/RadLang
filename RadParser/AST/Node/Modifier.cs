using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node;

public enum ModifierType {
  Unsigned
}

public class Modifier : Node<INode> {
  public ModifierType Type { get; internal set; }


  public Modifier(ParserRuleContext context) : base(context) {
  }
  
  public Modifier(ITerminalNode tokenNode) : base(tokenNode.Parent.RuleContext as ParserRuleContext) {
    var token = tokenNode.Payload as IToken;
    Text = tokenNode.GetText();
    Line = token.Line;
    Column = token.Column;
    Width = token.StopIndex;
  }
}
