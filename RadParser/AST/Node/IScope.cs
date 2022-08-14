namespace RadParser.AST.Node; 

public interface IScope : INode {
  List<Declaration> GetDeclarations();
  List<Declaration> GetAllInScopeDeclarations();
}
