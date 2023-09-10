using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node;

public class TypeIdentifier : Identifier {
  public List<Modifier> Modifiers { get; internal set; } = new();

  public TypeIdentifier(ITerminalNode? tokenNode) : base(tokenNode) {}
}
