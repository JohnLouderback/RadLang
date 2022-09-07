using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using RadParser;
using RadParser.AST.Node;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace RadLanguageServer.ASTVisitors;

public class DocumentSymbolASTVisitor : BaseASTVisitor {
  private readonly Stack<IList<DocumentSymbol>> Parents = new ();
  public DocumentSymbol RootSymbol { get; private set; }


  public override void Visit(Module node) {
    var children = new List<DocumentSymbol>();
    Parents.Push(children);
    base.Visit(node);
    Parents.Pop();
    var range = new Range(node.Line - 1, node.Column, node.EndLine - 1, node.EndColumn);
    RootSymbol = new DocumentSymbol {
      Name           = node.FilePath ?? "module",
      Kind           = SymbolKind.Module,
      Range          = range,
      SelectionRange = range,
      Children = children
    };
  }

  public override void Visit(FunctionDeclaration node) {
    // Get the current list of children.
    var parent = Parents.Peek();

    // Create a new list children
    var children = new List<DocumentSymbol>();
    // Set the latest parent to the current list of children.
    Parents.Push(children);
    // Visit all children.
    base.Visit(node);
    // Return to the previous parent scope.
    Parents.Pop();

    // Create a symbol representing this function declaration.
    var funcDeclSymbol = new DocumentSymbol {
      Name     = node.Identifier.Name,
      Kind     = SymbolKind.Function,
      Range    = new Range(node.Line - 1, node.Column, node.EndLine - 1, node.EndColumn),
      SelectionRange = new Range(
          node.Identifier.Line - 1,
          node.Identifier.Column,
          node.Identifier.EndLine - 1,
          node.Identifier.EndColumn
        ),
      Children = new Container<DocumentSymbol>()
    };

    // Add this symbol and all it's children to the original list of children.
    parent.Add(funcDeclSymbol);
  }
}
