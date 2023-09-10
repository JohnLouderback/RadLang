using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node;

public enum ModifierType {
  Unsigned
}

/// <summary>
///   Represents a modifier in the source code. For example, in the line
///   <c> var x: unsigned int = 5; </c>, <c> unsigned </c> is a modifier for the type <c> int </c>.
///   A modifier modifies a type or keyword, generally augmenting it in some way.
/// </summary>
/// <inheritdoc />
public class Modifier : TokenNode {
  public ModifierType Type { get; internal set; }

  public Modifier(ParserRuleContext context) : base(context) {}


  public Modifier(ITerminalNode? tokenNode) :
    base(tokenNode) {}
}
