using RadParser.AST.Node;
using RadParser.Constructs;

namespace RadParser.Utils;

public static class ASTNodeExtensions {
  /// <summary>
  ///   Returns whether this node is located at the same position as the cursor given.
  /// </summary>
  /// <param name="cursorPosition"> The cursor position to test against. </param>
  /// <returns> Returns <c> true </c> if this cursor is positioned at this node. </returns>
  public static bool ExistsAtCursorPosition(this INode node, Cursor cursorPosition) {
    var nodeLine      = node.Line;
    var nodeEndLine   = node.EndLine;
    var nodeColumn    = node.Column;
    var nodeEndColumn = node.EndColumn;

    // If the cursor is on the same line as this node and between the starting and ending columns.
    if (nodeLine == cursorPosition.Line &&
        nodeColumn <= cursorPosition.Column &&
        nodeEndColumn >= cursorPosition.Column) {
      return true;
    }

    // Else, if the node is multi-line, and if the cursor is on the starting line for
    // this node and after the  the starting column.
    if (nodeLine != nodeEndLine &&
        nodeLine == cursorPosition.Line &&
        nodeColumn <= cursorPosition.Column) {
      return true;
    }

    // Else, if the node is multi-line, and if the cursor is on the same line as the ending line for
    // this node and before the ending column.
    if (nodeLine != nodeEndLine &&
        nodeEndLine == cursorPosition.Line &&
        nodeEndColumn >= cursorPosition.Column) {
      return true;
    }

    // Else, if the node is multi-line, if the cursor is between the starting and ending lines for
    // this node.
    if (nodeLine != nodeEndLine &&
        nodeLine < cursorPosition.Line &&
        nodeEndLine > cursorPosition.Line) {
      return true;
    }

    // Otherwise, the cursor is not within this node.
    return false;
  }


  /// <summary>
  ///   Finds the declaration for a given reference. Returns <c> null </c> if not found.
  /// </summary>
  /// <param name="reference"> The reference to find the declaration for. </param>
  /// <typeparam name="T"> The type of reference. </typeparam>
  /// <returns> The declaration for the given reference or <c> null </c> if not found. </returns>
  public static Declaration? GetDeclaration<T>(this IReference<T> reference) where T : Identifier {
    return reference.GetParentScope()!.GetAllInScopeDeclarations()
      .Find(decl => decl.Identifier.Name == reference.Identifier.Name);
  }
}
