using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node;

/// <inheritdoc />
/// <summary>
///   <para>
///     The base class for all nodes that are represented by a single token in the source code.
///     Examples of this are <see cref="T:RadParser.AST.Node.Identifier" />,
///     <see cref="T:RadParser.AST.Node.Keyword" />, and
///     <see cref="T:RadParser.AST.Node.Reference" />.
///   </para>
///   <para>
///     This class should not be confused with <see cref="T:RadParser.AST.Node.Token" />. The
///     difference between the two is that <see cref="T:RadParser.AST.Node.Token" /> is used to
///     represent a token that serves no greater purpose other than syntax. For example, a colon in a
///     type specifier might be useful for syntax highlighting while serving no greater abstract
///     purpose. In the line: <c> var x: int = 5; </c>, the colon is a token that serves no other
///     purpose other than syntax.
///   </para>
///   <para>
///     <see cref="T:RadParser.AST.Node.TokenNode" /> is used to represent
///     nodes that are backed by a single token in the source code, but serve a greater purpose than
///     just syntax. For example, <see cref="T:RadParser.AST.Node.Identifier" /> is backed by a
///     single token in the source code, but serves a greater purpose than just syntax. It is used to
///     represent the name of a variable, function, type, etc.
///   </para>
/// </summary>
public abstract class TokenNode : Node<INode> {
  /// <summary>
  ///   Constructs a new <see cref="TokenNode" /> from the given <paramref name="context" />. This
  ///   is useful for when a node can be constructed from either a <see cref="ITerminalNode" /> or
  ///   a <see cref="ParserRuleContext" />.
  /// </summary>
  /// <param name="context"> The <see cref="ParserRuleContext" /> to construct the node from. </param>
  protected TokenNode(ParserRuleContext context) : base(context) {}


  /// <summary>
  ///   Constructs a new <see cref="TokenNode" /> from the given <paramref name="tokenNode" />.
  /// </summary>
  /// <param name="tokenNode">
  ///   The <see cref="ITerminalNode" /> to construct the <see cref="TokenNode" /> from.
  /// </param>
  protected TokenNode(ITerminalNode? tokenNode) :
    base(tokenNode?.Parent?.RuleContext as ParserRuleContext) {
    if (tokenNode?.Payload is not IToken token) return;
    Text   = tokenNode.GetText();
    Line   = token.Line;
    Column = token.Column;
    Width  = Text.Length;
  }
}
