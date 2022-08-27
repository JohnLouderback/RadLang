using Antlr4.Runtime;

namespace RadParser.AST.Node;

public abstract class Declaration : Node<INode> {
  public DeclaratorKeyword Keyword { get; internal set; }
  public Identifier Identifier { get; internal set; }

  public Declaration(ParserRuleContext context) : base(context) {}
}
