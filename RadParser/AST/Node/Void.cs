using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node;

/// <summary>
///   Represents the <c> void </c> keyword in the source code. This is a special type that is used
///   to represent the absence of a type altogether.
/// </summary>
/// <inheritdoc />
public class Void : TypeReference {
  public Void(ITerminalNode? tokenNode) : base(tokenNode) {}
}
