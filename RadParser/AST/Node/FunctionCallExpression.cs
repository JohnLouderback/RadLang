using Antlr4.Runtime;

namespace RadParser.AST.Node; 

public class FunctionCallExpression : Expression {
  public Reference Reference { get; internal set; }
  public List<PositionalParameter> Arguments { get; internal set; }


  public FunctionCallExpression(ParserRuleContext context) : base(context) {
  }
}
