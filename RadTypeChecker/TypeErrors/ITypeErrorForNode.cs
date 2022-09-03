using RadParser.AST.Node;

namespace RadTypeChecker.TypeErrors;

public interface ITypeErrorForNode<out T> where T : INode {
  T ForNode { get; }
}
