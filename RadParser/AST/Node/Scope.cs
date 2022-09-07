using Antlr4.Runtime;
using static RadParser.Utils.ASTUtils;

namespace RadParser.AST.Node;

public abstract class Scope<T> : Node<T>, IScope where T : class {
  public List<T> AllStatements {
    get => Children;
    set => Children = value;
  }

  public Scope(ParserRuleContext context) : base(context) {}


  /// <summary>
  ///   Gets a list of all declarations that are applicable within this scope. Basically, anything
  ///   lexically scoped.
  /// </summary>
  /// <returns> A list of all declarations that are applicable within this scope. </returns>
  public virtual List<Declaration> GetAllInScopeDeclarations() {
    var parentScope = GetParentScope();

    // If there is a parent scope, combine its declarations with the declarations of this scope.
    if (parentScope is not null) {
      return GetDeclarations().Concat(parentScope.GetAllInScopeDeclarations()).ToList();
    }

    // Otherwise, just use the declarations directly decalred within this scope.
    return GetDeclarations();
  }


  /// <summary>
  ///   Gets all nested references that are descendents in this scope. This does not return references in
  ///   any parents closer to the root.
  /// </summary>
  /// <returns> A list of all descendent / nested references in this scope. </returns>
  public virtual List<Reference> GetAllReferencesInScope() {
    var references = new List<Reference>();

    // Find all descendent references in this scope.
    WalkAST(
        this,
        (node, parentNode) => {
          if (node is Reference reference) references.Add(reference);
          return true;
        }
      );

    return references;
  }


  /// <summary>
  ///   Gets a list of all declarations that are declared directly in this scope.
  /// </summary>
  /// <returns> A list of all declarations that are declared directly in this scope. </returns>
  public virtual List<Declaration> GetDeclarations() {
    return AllStatements
             .Where(statement => ((dynamic)statement).Value is Declaration)
             .Select(statement => ((dynamic)statement).Value as Declaration)
             .ToList() as List<Declaration>;
  }
}
