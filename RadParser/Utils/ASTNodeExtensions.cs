using RadParser.AST.Node;

namespace RadParser.Utils;

public static class ASTNodeExtensions {
  public static Declaration? GetDeclaration<T>(this IReference<T> reference) where T : Identifier {
    foreach (var decl in reference.GetAncestorsOfType<Declaration>()) {
      // If this declaration matches this reference
      if ((decl.Identifier.Name == reference.Identifier.Name)) {
        return decl;
      }

      // Or, if this declaration is a function declaration, check the parameters as well.
      if (decl is FunctionDeclaration funcDecl) {
        foreach (var param in funcDecl.Parameters) {
          if (param.Identifier.Name == reference.Identifier.Name) {
            return param;
          }
        }
      }
    }
    
    return null;
  }
}
