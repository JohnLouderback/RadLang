using Antlr4.Runtime;
using RadParser.Utils.Attributes;

namespace RadParser.AST.Node;

public class FunctionScope : Scope<FunctionScopeStatement> {
  [Ancestral] public FunctionDeclaration Function => GetFirstAncestorOfType<FunctionDeclaration>();

  public FunctionScope(ParserRuleContext context) : base(context) {}


  public override List<Declaration> GetDeclarations() {
    return Function.Parameters.Concat(base.GetDeclarations()).ToList();
  }
}
