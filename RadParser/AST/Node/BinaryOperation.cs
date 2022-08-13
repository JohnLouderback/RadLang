using Antlr4.Runtime;
using RadParser.AST.Node;

namespace RadParser; 

public class BinaryOperation : Operation<int> {
  public Value LeftOperand { get; internal set; }
  public Value RightOperand { get; internal set; }
  public Operator Operator { get; internal set; } 

  public bool CanDetermineResult => LeftOperand.Value is Literal && RightOperand.Value is Literal;

  public int? Result {
    get {
      if (!CanDetermineResult) return default;
      if (LeftOperand.Value is NumericLiteral &&
          RightOperand.Value is NumericLiteral) {
        return Operator.Type switch {
          OperatorType.Star => (LeftOperand.Value as NumericLiteral)?.Value * (RightOperand.Value as NumericLiteral)?.Value,
          OperatorType.ForwardSlash => (LeftOperand.Value as NumericLiteral)?.Value / (RightOperand.Value as NumericLiteral)?.Value,
          OperatorType.Plus => (LeftOperand.Value as NumericLiteral)?.Value + (RightOperand.Value as NumericLiteral)?.Value,
          OperatorType.Minus => (LeftOperand.Value as NumericLiteral)?.Value - (RightOperand.Value as NumericLiteral)?.Value,
          _ => default
        };
      }

      return default;
    }
  }


  public BinaryOperation(ParserRuleContext context) : base(context) {
  }
}
