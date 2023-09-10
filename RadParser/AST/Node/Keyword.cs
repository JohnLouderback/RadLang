using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node;

/// <summary>
///   Represents a keyword in the source code. For example, in the line <c> var x = 5; </c>,
///   <c> var </c> is a keyword.
/// </summary>
/// <inheritdoc />
public abstract class Keyword : TokenNode {
  public Keyword(ParserRuleContext context) : base(context) {}


  public Keyword(ITerminalNode? tokenNode) :
    base(tokenNode) {}
}
