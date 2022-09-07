using Antlr4.Runtime;
using RadUtils.Constructs;

namespace RadParser.AST.Node;

public interface INode {
  /// <summary>
  ///   The parent node of this node in the tree. If the node is a module, it will have no parent
  ///   and return <c> null </c>.
  /// </summary>
  public INode Parent { get; set; }

  /// <summary>
  ///   The string text from the source code from which this node was generated.
  /// </summary>
  public string Text { get; }

  /// <summary>
  ///   The location in the source code this node represents.
  /// </summary>
  public SourceCodeLocation Location { get; }

  /// <summary>
  ///   The starting line that this node occurs on in the source code.
  /// </summary>
  public int Line { get; }

  /// <summary>
  ///   The starting column that this node occurs on in the source code.
  /// </summary>
  public int Column { get; }

  /// <summary>
  ///   The length in characters of this node as it exists in the source code.
  /// </summary>
  public int Width { get; }

  /// <summary>
  ///   An array of strings representing each line of this node in the source code.
  /// </summary>
  public string[] Lines { get; }

  /// <summary>
  ///   The ending line position for this node as it exists in the source code.
  /// </summary>
  public int EndLine { get; }

  /// <summary>
  ///   The final column position for the last line of this node as it exists in the source code.
  /// </summary>
  public int EndColumn { get; }

  /// <summary>
  ///   The node's counterpart in the concrete syntax tree.
  /// </summary>
  public ParserRuleContext CSTNode { get; }


  /// <summary>
  ///   Searches for instances of given type in the ancestors of this node and returns them, if found.
  /// </summary>
  /// <param name="checkSelf"> Whether the search should include checking this node itself. </param>
  /// <typeparam name="TAncestorType"> The type of node to search for. </typeparam>
  /// <returns> A list of nodes match the type provided. </returns>
  IEnumerable<TAncestorType> GetAncestorsOfType
    <TAncestorType>(bool checkSelf) where TAncestorType : class;


  /// <summary>
  ///   Searches for the first instance of the given type among the ancestors of this node and
  ///   returns it, if found. Returns <c> null </c> if not found.
  /// </summary>
  /// <param name="checkSelf"> Whether the search should include checking this node itself. </param>
  /// <typeparam name="TAncestorType"> The type of node to search for. </typeparam>
  /// <returns> The node matches the type provided, or <c> null </c> if not found. </returns>
  TAncestorType? GetFirstAncestorOfType<TAncestorType>(bool checkSelf) where TAncestorType : class;


  /// <summary>
  ///   Returns the <see cref="IScope" /> representing the lexical scope this node exists in.
  /// </summary>
  /// <returns> The node representing the lexical scope for the current node. </returns>
  IScope? GetParentScope();
}
