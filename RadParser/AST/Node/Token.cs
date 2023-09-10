using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node;

/// <summary>
///   <para>
///     Represents a concrete token for information purposes. For instance, a colon in a type
///     specifier might be useful for syntax highlighting while serving no greater abstract purpose.
///     In the line: <c> var x: int = 5; </c>, the colon is a token that serves no other purpose other
///     than syntax.
///   </para>
///   <para>
///     This class isn't to be confused with <see cref="TokenNode" />, which is an abstract base
///     class to be used by all node which are backed by a single terminal token. This class is
///     instead used specifically for tokens that serve no greater purpose other than syntax.
///   </para>
/// </summary>
/// <inheritdoc />
public class Token : TokenNode {
  public Token(ITerminalNode tokenNode) : base(tokenNode) {}
}
