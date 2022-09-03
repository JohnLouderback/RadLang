using RadParser;
using RadParser.AST.Node;
using RadParser.Utils;
using RadTypeChecker.TypeErrors;

namespace RadTypeChecker;

public class TypeCheckASTVisitor : BaseASTVisitor {
  /// <summary>
  ///   A list of type errors found while type checking the AST.
  /// </summary>
  public List<ITypeError> TypeErrors { get; } = new();


  public override void Visit(Reference node) {
    var declarationForRef = node.GetDeclaration();
    if (declarationForRef is null) {
      TypeErrors.Add(
          new UndefinedReferenceError(
              node,
              $"Could not find any in-scope declaration for identifier `{node.Identifier.Name}`."
            )
        );
    }
  }
}
