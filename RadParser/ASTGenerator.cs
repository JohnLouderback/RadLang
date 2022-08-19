using System.Reflection;
using RadLexer;
using RadParser.AST.Node;
using RadParser.Utils.Attributes;
using Module = RadParser.AST.Node.Module;

namespace RadParser;

public class ASTGenerator {
  public Module GenerateASTFromCST(Rad.StartRuleContext cst) {
    return Ophanarium(new ASTBuilderVisitor().VisitStartRule(cst));
  }


  /// <summary>
  ///   Finds all orphans nodes and gives them parent nodes.
  /// </summary>
  /// <param name="ast"> </param>
  /// <returns> </returns>
  public T Ophanarium<T>(T ast) where T : class, INode {
    var astNodeType = ast.GetType();

    // TODO  check the 2 objects have same type

    foreach (var prop in astNodeType.GetProperties(
                 BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy
               )) {
      var hasComputedAttribute =
        Attribute.GetCustomAttribute(prop, typeof(AncestoralAttribute)) is AncestoralAttribute;
      var propIsParent = prop.Name == "Prop";

      // If this prop is computed, we'll avoid checking it for nodes since they may be circular.
      // Or if this prop is the "Parent" prop, don't follow it upwards.
      if (hasComputedAttribute || propIsParent) continue;

      var value               = prop.GetValue(ast);
      var isNode              = prop.PropertyType.GetInterface(nameof(INode)) is not null;
      var isNodeList          = (value as IEnumerable<INode>) is not null;
      var isOneOfTypeWithNode = value?.GetType().GetProperty("Value")?.GetValue(value) is INode;

      // If the value is a `OneOf` type with a `.Value` property that is a node.
      if (isOneOfTypeWithNode) {
        value  = (value as dynamic)?.Value;
        isNode = value is INode;
      }

      // If the property represents a AST Node, assign its parent and recursively check its properties.
      if (isNode && value is not null) {
        var node                             = value as INode;
        if (node.Parent == null) node.Parent = ast;
        Ophanarium(node);
      }

      if (isNodeList) {
        if (prop.GetValue(ast) is not IEnumerable<INode> nodeList) {
          throw new Exception(
              $"{nameof(nodeList)} could not converted to an {nameof(IEnumerable<INode>)} interface."
            );
        }

        foreach (var node in nodeList) {
          if (node.Parent == null) node.Parent = ast;
          Ophanarium(node);
        }
      }
    }

    return ast;
  }
}
