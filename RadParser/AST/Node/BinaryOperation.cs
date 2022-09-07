using Antlr4.Runtime;
using RadParser.AST.Node;
using RadParser.AST.Traits;

namespace RadParser; 

public class BinaryOperation : Operation<int> {
  public Value LeftOperand { get; internal set; }
  public Value RightOperand { get; internal set; }
  public Operator Operator { get; internal set; }

  /// <summary>
  /// Can the result be executed at compile-time or "design-time"?
  /// </summary>
  public new bool CanDetermineResult => IsStaticConstant;

  /// <summary>
  /// The result of the operation, the values are constant.
  /// </summary>
  public int? Result {
    get {
      if (!CanDetermineResult) return default;
      if (LeftOperand.Value is NumericLiteral leftNumLiteral &&
          RightOperand.Value is NumericLiteral rightNumLiteral) {
        return Operator.Type switch {
          OperatorType.Star         => leftNumLiteral.Value * rightNumLiteral.Value,
          OperatorType.ForwardSlash => leftNumLiteral.Value / rightNumLiteral.Value,
          OperatorType.Plus         => leftNumLiteral.Value + rightNumLiteral.Value,
          OperatorType.Minus        => leftNumLiteral.Value - rightNumLiteral.Value,
          _                         => default
        };
      }

      return default;
    }
  }


  public BinaryOperation(ParserRuleContext context) : base(context) {
  }

  /// <inheritdoc />
  public override bool IsStaticConstant =>
    // If both the left operand is a possible constant whose `IsStaticConstant` property is true...
    LeftOperand.Value is IPossibleConstant { IsStaticConstant: true } &&
    // ...and the right operand is a possible constant whose `IsStaticConstant` property is true.
    RightOperand.Value is IPossibleConstant { IsStaticConstant: true }; // ...then this is a static constant.
}
