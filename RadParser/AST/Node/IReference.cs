using RadParser.Utils.Attributes;

namespace RadParser.AST.Node; 

public interface IReference<T> : INode where T : Identifier {
  public T Identifier { get; set; }
}
