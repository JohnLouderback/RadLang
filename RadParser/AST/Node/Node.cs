using Antlr4.Runtime;
using RadParser.Utils;

namespace RadParser.AST.Node;

public abstract class Node<T> : INode {
  public string Text { get; internal set; }
  public INode Parent { get; set; }

  public List<T> Children { get; internal set; } = new();

  public T? Child {
    get => Children.FirstOrDefault();
    set {
      Children.Clear();
      if (value is null) return;
      Children.Add(value);
    }
  }

  public int Line { get; internal set; }
  public int Column { get; internal set; }
  public int Width { get; internal set; }
  public ParserRuleContext CSTNode { get; internal set; }

  public Node(ParserRuleContext context) {
    Text = context.GetFullText();
    Line = context.Start.Line;
    Column = context.Start.Column;
    Width = context.Stop.StopIndex;
    CSTNode = context;
  }


  public IEnumerable<TAncestorType> GetAncestorsOfType<TAncestorType>() where TAncestorType : Node<INode> {
    var ancestors = new List<TAncestorType>();
    if (this is Module) {
      throw new Exception($"{nameof(Module)} cannot have ancestors.");
    }
    
    if (Parent is null) {
      throw new Exception(
        $"{nameof(Parent)} is null for {this.GetType().Name} \"{Text}\". It's possible the AST is being accessed prior to full generation."
      );
    }

    if (Parent is TAncestorType) {
      ancestors.Add(Parent as TAncestorType);
      return ancestors;
    }

    if (Parent is not Module) {
      return ancestors.Concat((Parent as Node<INode>)!.GetAncestorsOfType<TAncestorType>());
    }

    return ancestors;
  }
}
