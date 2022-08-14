using Antlr4.Runtime;

namespace RadParser.AST.Node;

public class FunctionDeclaration : Declaration {
  public List<NamedTypeParameter> Parameters { get; internal set; }
  public TypeReference ReturnType { get; internal set; }
  public FunctionScope Statements { get; internal set; }


  public FunctionDeclaration(ParserRuleContext context) : base(context) {
  }
}
