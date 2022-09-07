using Antlr4.Runtime;

namespace RadParser.AST.Node;

public class LiteralExpression : Expression {
  public Literal Literal { get; internal set; }

  /// <inheritdoc />
  public override bool IsStaticConstant => Literal.IsStaticConstant;

  public LiteralExpression(ParserRuleContext context) : base(context) {}


  public static implicit operator Literal(LiteralExpression literalExp) {
    return literalExp.Literal;
  }
}
