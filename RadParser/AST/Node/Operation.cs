using Antlr4.Runtime;

namespace RadParser.AST.Node; 

public class Operation<T> : Expression {
  public bool CanDetermineResult { get; } = false;
  public T? Result { get; } = default;


  public Operation(ParserRuleContext context) : base(context) {
  }
}
