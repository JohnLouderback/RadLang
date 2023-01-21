using RadParser.AST.Node;

namespace RadTypeChecker.TypeErrors;

/// <summary>
///   Represents a type error which occurs when the current function call is not passed the correct number of arguments
///   expected.
/// </summary>
public class IncorrectNumberOfArgumentsError : TypeError<FunctionCallExpression> {
  /// <inheritdoc />
  public IncorrectNumberOfArgumentsError(FunctionCallExpression node, string message) : base(
      node,
      message
    ) {}
}
