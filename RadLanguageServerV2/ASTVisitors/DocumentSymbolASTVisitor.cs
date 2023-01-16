using Microsoft.VisualStudio.LanguageServer.Protocol;
using RadParser;
using RadParser.AST.Node;
using Range = Microsoft.VisualStudio.LanguageServer.Protocol.Range;

namespace RadLanguageServerV2.ASTVisitors;

public class DocumentSymbolASTVisitor : BaseASTVisitor {
  private readonly Stack<IList<DocumentSymbol>> Parents = new();
  public DocumentSymbol RootSymbol { get; private set; }


  public override void Visit(Module node) {
    var children = new List<DocumentSymbol>();
    Parents.Push(children);
    base.Visit(node);
    Parents.Pop();
    var range = new Range {
      Start = new Position {
        Line      = node.Line - 1,
        Character = node.Column
      },
      End = new Position {
        Line      = node.EndLine - 1,
        Character = node.EndColumn
      }
    };
    RootSymbol = new DocumentSymbol {
      Name           = node.FilePath ?? "module",
      Kind           = SymbolKind.Module,
      Range          = range,
      SelectionRange = range,
      Children       = children.ToArray()
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
      Name = node.Identifier.Name,
      Kind = SymbolKind.Function,
      Range = new Range {
        Start = new Position {
          Line      = node.Line - 1,
          Character = node.Column
        },
        End = new Position {
          Line      = node.EndLine - 1,
          Character = node.EndColumn
        }
      },
      SelectionRange = new Range {
        Start = new Position {
          Line      = node.Identifier.Line - 1,
          Character = node.Identifier.Column
        },
        End = new Position {
          Line      = node.Identifier.EndLine - 1,
          Character = node.Identifier.EndColumn
        }
      },
      Children = Array.Empty<DocumentSymbol>()
    };

    // Add this symbol and all it's children to the original list of children.
    parent.Add(funcDeclSymbol);
  }
}
