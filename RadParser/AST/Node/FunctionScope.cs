using Antlr4.Runtime;
using RadParser.Utils.Attributes;

namespace RadParser.AST.Node; 

public class FunctionScope : Scope<FunctionScopeStatement> {
  [Ancestoral]
  public FunctionDeclaration Function => GetFirstAncestorOfType<FunctionDeclaration>();

  public override List<Declaration> GetDeclarations() => Function.Parameters.Concat(base.GetDeclarations()).ToList();


  public FunctionScope(ParserRuleContext context) : base(context) {
    
  }
}
