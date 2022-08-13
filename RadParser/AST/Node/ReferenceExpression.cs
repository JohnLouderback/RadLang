using Antlr4.Runtime;

namespace RadParser.AST.Node; 

public class ReferenceExpression : Expression {
  public Identifier Identifier => Reference.Identifier;
  public Reference Reference { get; internal set; }
  
  public ReferenceExpression(ParserRuleContext context) : base(context) {
  }
  
  public static implicit operator Identifier(ReferenceExpression referenceExp) => referenceExp.Identifier;
  public static implicit operator Reference(ReferenceExpression referenceExp) => referenceExp.Reference;
}
