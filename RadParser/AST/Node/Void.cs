using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node; 

public class Void : TypeReference {
  public Identifier? Identifier => null;
  public Void(ITerminalNode tokenNode) : base(tokenNode) {
  }
}
