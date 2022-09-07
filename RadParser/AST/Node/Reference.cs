using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using RadParser.AST.Traits;
using RadParser.Utils;
using RadParser.Utils.Attributes;

namespace RadParser.AST.Node; 

public class Reference : Node<INode>, IReference<Identifier>, IPossibleConstant {
  public virtual Identifier Identifier { get; set; }

  public Reference(ITerminalNode tokenNode) : base(tokenNode.Parent.RuleContext as ParserRuleContext) {
    var token = tokenNode.Payload as IToken;
    Text = tokenNode.GetText();
    Line = token.Line;
    Column = token.Column;
    Width = Text.Length;
  }


  /// <inheritdoc />
  public bool IsStaticConstant => (this as IReference<Identifier>).GetDeclaration()?.IsStaticConstant ?? false;
}
