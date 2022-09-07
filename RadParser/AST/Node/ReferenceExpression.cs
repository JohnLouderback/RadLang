using Antlr4.Runtime;

namespace RadParser.AST.Node;

public class ReferenceExpression : Expression {
  public Identifier Identifier => Reference.Identifier;
  public Reference Reference { get; internal set; }

  /// <inheritdoc />
  public override bool IsStaticConstant => Reference.IsStaticConstant;

  public ReferenceExpression(ParserRuleContext context) : base(context) {}


  public static implicit operator Identifier(ReferenceExpression referenceExp) {
    return referenceExp.Identifier;
  }


  public static implicit operator Reference(ReferenceExpression referenceExp) {
    return referenceExp.Reference;
  }
}
