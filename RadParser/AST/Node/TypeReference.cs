using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node;

public class TypeReference : Token, IReference<TypeIdentifier> {
  public TypeIdentifier Identifier { get; set; }
  public TypeReference(ITerminalNode tokenNode) : base(tokenNode) {}
}
