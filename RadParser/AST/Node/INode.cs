using Antlr4.Runtime;

namespace RadParser.AST.Node; 

public interface INode {
  public INode Parent { get; set; }
  public string Text { get; }
  public int Line { get; }
  public int Column { get; }
  public int Width { get; }
  public ParserRuleContext CSTNode { get; }
  IEnumerable<TAncestorType> GetAncestorsOfType<TAncestorType>() where TAncestorType : Node<INode>;
}
