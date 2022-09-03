using RadParser.AST.Node;
using RadTypeChecker.TypeErrors;

namespace RadTypeChecker;

/// <summary>
///   This class represents a "Type Checker" that can be used to verify that there are no type errors
///   in an AST for a given document. For instance: all variable references are declared, or that
///   arguments match their parameter types.
/// </summary>
public class TypeChecker {
  /// <summary>
  ///   Type checks an AST tree, given the root node.
  /// </summary>
  /// <param name="astRoot"> The root AST node for the tree. </param>
  /// <returns>
  ///   A list of type errors. If the list in empty, then type checking succeeded without errors.
  /// </returns>
  public List<ITypeError> CheckTypes(INode astRoot) {
    var visitor = new TypeCheckASTVisitor();
    visitor.Visit(astRoot);
    return visitor.TypeErrors;
  }
}
