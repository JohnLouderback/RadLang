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

  public TAncestorType? GetFirstAncestorOfType<TAncestorType>(bool checkSelf = false) where TAncestorType : class {
    // If this node should check to see if itself is the type.
    if (checkSelf && this is TAncestorType) {
      return this as TAncestorType;
    }
    
    if (this is Module) {
      throw new Exception($"{nameof(Module)} cannot have ancestors.");
    }
    
    if (Parent is null) {
      throw new Exception(
        $"{nameof(Parent)} is null for {this.GetType().Name} \"{Text}\". It's possible the AST is being accessed prior to full generation."
      );
    }

    if (Parent is TAncestorType node) {
      return node;
    }

    // If the parent is not "Module" (The root element), then keep looking. Otherwise, give up.
    return Parent is not Module ? (Parent as INode)!.GetFirstAncestorOfType<TAncestorType>(true) : null;
  }

  public IEnumerable<TAncestorType> GetAncestorsOfType<TAncestorType>(bool checkSelf = false) where TAncestorType : class {
    var ancestors = new List<TAncestorType>();

    var ancestor = GetFirstAncestorOfType<TAncestorType>(checkSelf);

    if (ancestor is INode node &&
        node.Parent is not Module) {
      return ancestors.Concat((Parent as Node<INode>)!.GetAncestorsOfType<TAncestorType>(true));
    }

    return ancestors;
  }


  public IScope? GetParentScope() {
    if (this is Module or TopLevel) {
      return null;
    }
    return GetFirstAncestorOfType<IScope>();
  }
}
