using RadParser.AST.Node;

namespace RadTypeChecker.TypeErrors;

public class UnknownTypeError : TypeError<TypeReference> {
  /// <inheritdoc />
  public UnknownTypeError(TypeReference node, string message) : base(node, message) {}
}
