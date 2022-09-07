using Antlr4.Runtime;
using RadParser.AST.Traits;

namespace RadParser.AST.Node;

public class PositionalParameter : Node<Value>, IPossibleConstant {
  public Value Value {
    get => Child!;
    set => Children = new List<Value> { value };
  }

  /// <inheritdoc />
  public bool IsStaticConstant =>
    // If the value is a possible constant whose `IsStaticConstant` property is true.
    Value.Value is IPossibleConstant { IsStaticConstant: true };

  public PositionalParameter(ParserRuleContext context) : base(context) {}
}
