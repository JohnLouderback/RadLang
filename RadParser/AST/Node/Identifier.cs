using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node;

/// <inheritdoc />
/// <summary>
///   Represents an identifier in the source code. For example, in the line <c> var x = 5; </c>,
///   <c> x </c> is an identifier.
/// </summary>
public class Identifier : TokenNode {
  /// <summary>
  ///   The name of the identifier. For example, in the line <c> var x = 5; </c>, <c> x </c> is the name
  ///   of the identifier.
  /// </summary>
  public string Name { get; internal set; }


  /// <summary>
  ///   Constructs a new <see cref="Identifier" /> from the given <paramref name="tokenNode" />.
  /// </summary>
  /// <param name="tokenNode">
  ///   The <see cref="ITerminalNode" /> to construct the <see cref="Identifier" /> from.
  /// </param>
  /// <inheritdoc cref="TokenNode" />
  public Identifier(ITerminalNode? tokenNode) : base(
      tokenNode
    ) {
    Name = Text;
  }
}
