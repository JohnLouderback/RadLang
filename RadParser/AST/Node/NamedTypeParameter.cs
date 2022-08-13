using Antlr4.Runtime;

namespace RadParser.AST.Node; 

public class NamedTypeParameter : Declaration {
  public TypeReference TypeReference { get; internal set; }
  
  public NamedTypeParameter(ParserRuleContext context) : base(context) {
  }
}
