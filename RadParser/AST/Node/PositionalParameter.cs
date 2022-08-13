using Antlr4.Runtime;

namespace RadParser.AST.Node; 

public class PositionalParameter : Node<Value> {
  public Value Value {
    get => Child!;
    set => Children = new List<Value> { value };
  }


  public PositionalParameter(ParserRuleContext context) : base(context) {
  }
}
