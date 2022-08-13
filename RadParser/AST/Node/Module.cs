using Antlr4.Runtime;

namespace RadParser.AST.Node; 

public class Module : Node<TopLevel> {
  public string FilePath { get; internal set; }
  public TopLevel TopLevel {
    get => Child!;
    set => Child = value;
  }


  public Module(ParserRuleContext context) : base(context) {
  }
}
