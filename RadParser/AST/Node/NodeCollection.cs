using Antlr4.Runtime;

namespace RadParser.AST.Node; 

public class NodeCollection<T> : Node<T> {
  public NodeCollection(ParserRuleContext context) : base(context) {
  }

  public List<T> ToList() {
    return Children;
  }

  public static implicit operator List<T>(NodeCollection<T> nodeCollection) => nodeCollection.ToList();
}
