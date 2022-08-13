using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node; 

public class Identifier : Node<INode> {
    public string Name { get; internal set; }


    public Identifier(ITerminalNode tokenNode) : base(tokenNode.Parent.RuleContext as ParserRuleContext) {
        var token = tokenNode.Payload as IToken;
        Text = tokenNode.GetText();
        Name = Text;
        Line = token.Line;
        Column = token.Column;
        Width = token.StopIndex;
    }
}
