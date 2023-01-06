using RadParser.AST.Node;

namespace RadTypeChecker.TypeErrors;

public class UnknownTypeError : TypeError<TypeReference> {
  /// <inheritdoc />
  public override uint ErrorCode { get; } = 2;


  /// <inheritdoc />
  public UnknownTypeError(TypeReference node, string message) : base(node, message) {}
}
