using Antlr4.Runtime;

namespace RadParser.AST.Node; 

public abstract class Declaration : Node<INode> {
  public Identifier Identifier { get; internal set; }
  public Declaration(ParserRuleContext context) : base(context) {
  }
}
