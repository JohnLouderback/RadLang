using Antlr4.Runtime;

namespace RadParser.AST.Node; 

public class NamedTypeParameter : Declaration {
  public TypeReference TypeReference { get; internal set; }
  
  public NamedTypeParameter(ParserRuleContext context) : base(context) {
  }


  /// <inheritdoc />
  public override bool IsStaticConstant =>
    false; // Parameters are variable by their very nature, so they cannot be considered static constants.

}
