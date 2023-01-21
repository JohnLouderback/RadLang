using Antlr4.Runtime;
using RadParser.Utils;
using RadUtils.Constructs;

namespace RadParser.AST.Node;

public abstract class Node<T> : INode {
  private static int LastKnownLineNumber = 1;
  private static int LastKnownColumnNumber;

  /// <inheritdoc />
  public string Text { get; internal set; }

  /// <inheritdoc />
  public SourceCodeLocation Location => new() {
    Line      = Line,
    EndLine   = EndLine,
    Column    = Column,
    EndColumn = EndColumn
  };

  /// <inheritdoc />
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

  /// <inheritdoc />
  public int Line { get; internal set; }

  /// <inheritdoc />
  public int Column { get; internal set; }

  /// <inheritdoc />
  public int Width { get; internal set; }

  /// <inheritdoc />
  public string[] Lines => Text.Split('\n');

  /// <inheritdoc />
  public int EndLine => Line + Lines.Length - 1;

  /// <inheritdoc />
  public int EndColumn => Line == EndLine ? Column + Width : Lines[^1].Length;

  /// <inheritdoc />
  public ParserRuleContext? CSTNode { get; internal set; }


  public Node(ParserRuleContext context) {
    // Context being `null` usually indicates a syntax error. We try to recover as well as we can.
    if (context is null) {
      Text    = "";
      Line    = LastKnownLineNumber;
      Column  = LastKnownColumnNumber;
      Width   = 0;
      CSTNode = null;
    }
    // Otherwise, proceed as normal.
    else {
      Text                  = context.GetFullText();
      Line                  = context.Start.Line;
      Column                = context.Start.Column;
      Width                 = Text.Length;
      CSTNode               = context;
      LastKnownLineNumber   = Line;
      LastKnownColumnNumber = Column + Width;
    }
  }


  /// <inheritdoc />
  public IEnumerable<TAncestorType> GetAncestorsOfType
    <TAncestorType>(bool checkSelf = false) where TAncestorType : class {
    var ancestors = new List<TAncestorType>();

    var ancestor = GetFirstAncestorOfType<TAncestorType>(checkSelf);

    if (ancestor is INode node &&
        node.Parent is not Module) {
      return ancestors.Concat((Parent as Node<INode>)!.GetAncestorsOfType<TAncestorType>(true));
    }

    return ancestors;
  }


  /// <inheritdoc />
  public TAncestorType? GetFirstAncestorOfType
    <TAncestorType>(bool checkSelf = false) where TAncestorType : class {
    // If this node should check to see if itself is the type.
    if (checkSelf && this is TAncestorType) {
      return this as TAncestorType;
    }

    if (this is Module) {
      throw new Exception($"{nameof(Module)} cannot have ancestors.");
    }

    if (Parent is null) {
      throw new Exception(
          $"{
            nameof(Parent)
          } is null for {
            GetType().Name
          } \"{
            Text
          }\". It's possible the AST is being accessed prior to full generation."
        );
    }

    if (Parent is TAncestorType node) {
      return node;
    }

    // If the parent is not "Module" (The root element), then keep looking. Otherwise, give up.
    return Parent is not Module ? Parent!.GetFirstAncestorOfType<TAncestorType>(true) : null;
  }


  /// <inheritdoc />
  public IScope? GetParentScope() {
    if (this is Module or TopLevel) {
      return null;
    }

    return GetFirstAncestorOfType<IScope>();
  }
}
