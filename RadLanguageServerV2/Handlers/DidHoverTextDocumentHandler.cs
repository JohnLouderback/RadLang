using Microsoft.VisualStudio.LanguageServer.Protocol;
using RadLanguageServerV2.Exceptions;
using RadLanguageServerV2.Services;
using RadParser.AST.Traits;
using RadParser.Utils;
using RadUtils.Constructs;
using Range = Microsoft.VisualStudio.LanguageServer.Protocol.Range;

namespace RadLanguageServerV2.Handlers;

public class DidHoverTextDocumentHandler : IRequestHandler<TextDocumentPositionParams, Hover?> {
  private readonly DocumentManagerService documentManagerService;


  public DidHoverTextDocumentHandler(DocumentManagerService documentManagerService) {
    this.documentManagerService = documentManagerService;
  }


  public async Task<Hover?> Handler(TextDocumentPositionParams args) {
    var textDocument = args.TextDocument;

    var content = documentManagerService.Documents[textDocument.Uri] ??
                  throw new TextDocumentNotFoundException(args.TextDocument.Uri);

    var cursorPosition = new Cursor {
      Line   = (uint)args.Position.Line + 1,
      Column = (uint)args.Position.Character + 1
    };
    // Get the most specific node at the cursor position.
    var node = ASTUtils.MostSpecificNodeAtCursorPosition(content.AST, cursorPosition);

    if (node is IDocumented documented) {}
    else {
      return null;
    }

    var hoverDebugString =
      $"* Line: {args.Position.Line + 1}\n* Col: {args.Position.Character + 1}\n\n```rad\n{node.Text}\n```";

    // Return the hover information.
    return new Hover {
      Contents = new MarkupContent {
        Kind  = MarkupKind.Markdown,
        Value = documented.Documentation.Markdown
      },
      Range = new Range {
        Start = new Position(node.Line - 1, node.Column),
        End   = new Position(node.EndLine - 1, node.EndColumn)
      }
    };
  }
}
