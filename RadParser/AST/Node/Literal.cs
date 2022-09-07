using Antlr4.Runtime;
using RadParser.AST.Traits;

namespace RadParser.AST.Node;

public abstract class Literal : Node<INode>, IPossibleConstant {
  /// <inheritdoc />
  public abstract bool IsStaticConstant { get; }

  public Literal(ParserRuleContext context) : base(context) {}
}
