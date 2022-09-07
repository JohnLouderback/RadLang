using Antlr4.Runtime;
using RadParser.AST.Traits;

namespace RadParser.AST.Node;

public abstract class Declaration : Node<INode>, IPossibleConstant {
  public DeclaratorKeyword Keyword { get; internal set; }
  public Identifier Identifier { get; internal set; }

  /// <inheritdoc />
  public abstract bool IsStaticConstant { get; }

  public Declaration(ParserRuleContext context) : base(context) {}
}
