using Antlr4.Runtime;

namespace RadParser.AST.Node;

public class Scope<T> : Node<T> {
  public List<Declaration> Declarations =>
    AllStatements
      .Where(statement => ((dynamic)statement).Value is Declaration)
      .Select(statement => ((dynamic)statement).Value as Declaration)
      .ToList() as List<Declaration>;

  public List<T> AllStatements {
    get => Children;
    set => Children = value;
  }


  public Scope(ParserRuleContext context) : base(context) {
  }
}
