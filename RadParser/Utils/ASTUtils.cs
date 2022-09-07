using System.Reflection;
using OneOf;
using RadParser.AST.Node;
using RadParser.Utils.Attributes;
using RadUtils.Constructs;

namespace RadParser.Utils;

/// <summary>
///   Utilities that apply to the AST as a whole. Differs from <see cref="ASTNodeExtensions" /> in that
///   it provides utilities for the whole tree rather than any specific node.
/// </summary>
public static class ASTUtils {
  /// <summary>
  ///   Finds the most specific node at the cursor position.
  /// </summary>
  /// <param name="astRoot"> The root of the AST to search. </param>
  /// <param name="cursorPosition"> The position of the cursor in the document. </param>
  /// <returns>
  ///   The most specific node at the specified position, or <c> null </c> if no node was found.
  /// </returns>
  public static INode? MostSpecificNodeAtCursorPosition(INode astRoot, Cursor cursorPosition) {
    // All of the nodes that are within the cursor position.
    var matchingNodes = new HashSet<INode>();

    // Traverse the AST searching for the most specific node within the cursor position.
    WalkAST(
        astRoot,
        (node, parent) => {
          // If this node is not within the cursor position, there's no reason to check its children.
          if (!node.ExistsAtCursorPosition(cursorPosition)) return false;

          // Otherwise, this node is a candidate for being the most specific node at the cursor position.
          matchingNodes.Add(node);
          return true;
        }
      );

    // If no nodes were found, return nothing.
    if (matchingNodes.Count == 0) return null;

    // Find the shortest text string length. The most specific node will be the shortest one.
    var minLength = matchingNodes.Min(node => node.Text.Length);
    // Return the node whose text length matches the shortest length previously found.
    return matchingNodes.FirstOrDefault(node => node.Text.Length == minLength);
  }


  /// <summary>
  ///   Finds all orphans nodes and gives them parent nodes.
  /// </summary>
  /// <param name="ast"> </param>
  /// <returns> </returns>
  public static T Ophanarium<T>(T ast) where T : class, INode {
    // traverse the AST tree recursively.
    return WalkAST(
        ast,
        (node, parent) => {
          // When a node is encountered, assign it its parent, if it doesn't already have one.
          node.Parent ??= parent;
          // Return true to indicate that the traversal should continue down this branch.
          return true;
        }
      );
  }


  /// <summary>
  ///   Traverses the AST tree recursively and calls the <c> callback </c> function for each node in the
  ///   tree. It will attempt to avoid traversing any properties on the node that may refer to
  ///   ancestor nodes.
  /// </summary>
  /// <param name="ast"> The AST node to start the traversal from. </param>
  /// <param name="callback">
  ///   A callback whose first parameter is the current node, whose second parameter is the parent
  ///   node, and whose return value is a boolean to determine whether the function should continue down
  ///   the branch its currently on.
  /// </param>
  /// <typeparam name="T"> The type of the AST node to start from. </typeparam>
  /// <returns> Returns the AST node passed in. </returns>
  /// <exception cref="Exception"> Throws an exception if the traversal fails. </exception>
  public static T WalkAST<T>(T ast, Func<INode, INode, bool> callback) where T : class, INode {
    // Get the type of the current node.
    var astNodeType = ast.GetType();
    var properties = astNodeType.GetProperties(
        BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy
      );

    // Iterate over every property in this node and determine if it is a child node.
    foreach (var prop in properties) {
      // Determines whether the property is annotated with `AncestoralAttribute`, indicating that
      // the property is an ancestor node and we shouldn't walk it.
      var hasAncestoralAttribute =
        Attribute.GetCustomAttribute(prop, typeof(AncestoralAttribute)) is AncestoralAttribute;
      // Check if this is the `Parent` property so we can ignore it.
      var propIsParent = prop.Name == "Prop";

      // If this prop is an ancestor, we'll avoid checking it for nodes since they are likely circular.
      // Or if this prop is the "Parent" prop, don't follow it upwards.
      if (hasAncestoralAttribute || propIsParent) continue;

      // Get the value of this property and determine its type. It may be an `INode`, a list of nodes
      // or a `OneOf` type whose `.Value` property is a node.
      var value      = prop.GetValue(ast);
      var isNode     = prop.PropertyType.GetInterface(nameof(INode)) is not null;
      var isNodeList = value is IEnumerable<INode>;
      var isOneOfTypeWithNode =
        new Lazy<bool>(
            () => value?.GetType().GetProperty("Value")?.GetValue(value) is INode
          );
      var isOneOfListWithNode =
        new Lazy<bool>(
            () =>
              value is IEnumerable<object> oneOfList &&
              oneOfList.Count() > 0 &&
              oneOfList.ToList()[0] is IOneOf oneOf &&
              oneOf.GetType()
                .GetProperty
                  ("Value")
                ?.GetValue(oneOf) is INode
          );

      // If the value is a `OneOf` type whose `.Value` property that is a node.
      if (isOneOfTypeWithNode.Value) {
        value  = (value as IOneOf)?.Value;
        isNode = value is INode;
      }

      // If the property represents a AST Node, assign its parent and recursively check its properties.
      if (isNode && value is not null) {
        // Get the value as a child of this node.
        var node = value as INode;

        // Call the callback. If it returns `true`, then continue traversing this branch.
        if (callback(node, ast)) {
          // Check this node's children for orphans.
          WalkAST(node, callback);
        }

        continue;
      }

      // If the property represents a list of AST Nodes, recursively check each node.
      if (isNodeList || isOneOfListWithNode.Value) {
        IEnumerable<INode> nodeList = null;
        if (isNodeList && prop.GetValue(ast) is IEnumerable<INode> asNodeList) {
          nodeList = asNodeList;
        }

        // If this is a list of `oneOf` types that include `INode`s as values, convert it to a node list.
        if (!isNodeList &&
            isOneOfListWithNode.Value &&
            value is IEnumerable<IOneOf> oneOfList) {
          nodeList = oneOfList.Select(oneOf => oneOf.Value as INode).ToList();
        }

        if (nodeList is null) {
          throw new Exception(
              $"{nameof(nodeList)} could not converted to an {nameof(IEnumerable<INode>)} interface."
            );
        }

        // Iterate over each child and search for orphans.
        foreach (var node in nodeList) {
          // Call the callback. If it returns `true`, then continue traversing this branch.
          if (callback(node, ast)) {
            // Check this node's children for orphans.
            WalkAST(node, callback);
          }
        }
      }
    }

    return ast;
  }
}
