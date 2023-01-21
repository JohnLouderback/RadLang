using RadParser.AST.Node;

namespace RadTypeChecker.TypeErrors;

/// <summary>
///   Represents a type error which occurs when the current identifier is a reference that cannot be resolved. Examples
///   include typos in identifiers or a declaration that has not been defined or imported.
/// </summary>
public class UndefinedReferenceError : TypeError<Reference> {
  /// <inheritdoc />
  public UndefinedReferenceError(Reference node, string message) : base(node, message) {}
}
