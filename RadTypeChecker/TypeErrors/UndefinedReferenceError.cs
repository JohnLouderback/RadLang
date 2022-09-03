using RadParser.AST.Node;

namespace RadTypeChecker.TypeErrors;

public class UndefinedReferenceError : TypeError<Reference> {
  /// <inheritdoc />
  public override uint ErrorCode { get; } = 1;


  /// <inheritdoc />
  public UndefinedReferenceError(Reference node, string message) : base(node, message) {}
}
