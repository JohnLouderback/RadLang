namespace RadParser.AST.Traits;

/// <summary>
///   Represents a node which may possibly be a static constant. That is, a type whose value is
///   statically known at compile time.
/// </summary>
public interface IPossibleConstant {
  /// <summary>
  ///   Whether this node represents a value that is statically known at compile time.
  /// </summary>
  bool IsStaticConstant { get; }
}
