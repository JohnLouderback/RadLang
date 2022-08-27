using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node;

public class TypeReference : Node<INode>, IReference<TypeIdentifier> {
  public TypeIdentifier Identifier { get; set; }


  public TypeReference(ITerminalNode tokenNode) : base(
      tokenNode.Parent.RuleContext as ParserRuleContext
    ) {
    var token = tokenNode.Payload as IToken;
    Text   = tokenNode.GetText();
    Line   = token.Line;
    Column = token.Column;
    Width  = Text.Length;
  }
}
