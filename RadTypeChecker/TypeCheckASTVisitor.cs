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
    // Check whether this reference refers to a previously made declaration.
    if (declarationForRef is null) {
      TypeErrors.Add(
          new UndefinedReferenceError(
              node,
              $"Could not find any in-scope declaration for identifier `{node.Identifier.Name}`."
            )
        );
    }

    base.Visit(node);
  }


  public override void Visit(FunctionCallExpression node) {
    if (node.Reference?.GetDeclaration() is FunctionDeclaration funcDecl) {
      // The number of parameters the function defines.
      var paramCount = funcDecl.Parameters.Count;
      // The number of arguments being passed to the function.
      var argCount = node.Arguments.Count;
      if (argCount != paramCount) {
        TypeErrors.Add(
            new IncorrectNumberOfArgumentsError(
                node,
                $"`{node.Reference.Identifier.Name}` has {paramCount} parameters defined, but was passed {argCount} arguments."
              )
          );
      }
    }
  }
}
