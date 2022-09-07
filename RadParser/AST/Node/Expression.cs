using Antlr4.Runtime;
using RadParser.AST.Traits;

namespace RadParser.AST.Node;

public abstract class Expression : Node<INode>, IPossibleConstant {
  /// <inheritdoc />
  public abstract bool IsStaticConstant { get; }

  public Expression(ParserRuleContext context) : base(context) {}
}
