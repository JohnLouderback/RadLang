using Antlr4.Runtime;

namespace RadParser.AST.Node;

/// <summary>
///   Represents an empty or incomplete expression.
/// </summary>
public class EmptyExpression : Expression {
  public override bool IsStaticConstant => false;
  public EmptyExpression(ParserRuleContext context) : base(context) {}
}
