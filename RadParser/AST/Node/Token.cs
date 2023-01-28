using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node;

/// <summary>
///   Represents a concrete token for information purposes. For instance, a colon in a type specifier might be useful for
///   syntax highlighting while serving no greater abstract purpose.
/// </summary>
public class Token : Node<INode> {
  public Token(ITerminalNode tokenNode) : base(
      tokenNode?.Parent?.RuleContext as ParserRuleContext
    ) {
    // A syntax error may cause the token to be omitted - make sure it is not null. If it is null,
    // the base constructor will handle the fallback behavior.
    if (tokenNode is null) return;

    var token = tokenNode.Payload as IToken;
    Text   = tokenNode.GetText();
    Line   = token.Line;
    Column = token.Column;
    Width  = Text.Length;
  }
}
