using Antlr4.Runtime;
using RadParser.Utils.Attributes;

namespace RadParser.AST.Node;

public abstract class Scope<T> : Node<T>, IScope where T : class {
  public virtual List<Declaration> GetDeclarations() =>
    AllStatements
      .Where(statement => ((dynamic)statement).Value is Declaration)
      .Select(statement => ((dynamic)statement).Value as Declaration)
      .ToList() as List<Declaration>;


  public virtual List<Declaration> GetAllInScopeDeclarations() {
    var parentScope = GetParentScope();
    if (parentScope is not null) {
      return GetDeclarations().Concat(parentScope.GetAllInScopeDeclarations()).ToList();
    }

    return GetDeclarations();
  }

  public List<T> AllStatements {
    get => Children;
    set => Children = value;
  }


  public Scope(ParserRuleContext context) : base(context) {
  }
}
