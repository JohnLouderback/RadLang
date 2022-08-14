using RadParser.AST.Node;

namespace RadParser.Utils;

public static class ASTNodeExtensions {
  public static Declaration? GetDeclaration<T>(this IReference<T> reference) where T : Identifier {
    return reference.GetParentScope()!.GetAllInScopeDeclarations().Find(decl => decl.Identifier.Name == reference.Identifier.Name);
  }
}
