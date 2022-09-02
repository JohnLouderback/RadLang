using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using RadParser.AST.Node;
using RadParser.Constructs;
using RadParser.Utils;

namespace RadLanguageServer;

public class HoverHandler : HoverHandlerBase {
  private readonly ILogger<HoverHandler> logger;
  private readonly DocumentManager documentManager;


  public HoverHandler(
    ILogger<HoverHandler> logger,
    DocumentManager documentManager
  ) {
    this.logger          = logger;
    this.documentManager = documentManager;
  }


  public override async Task<Hover?> Handle(
    HoverParams request,
    CancellationToken cancellationToken
  ) {
    var content = documentManager.Documents[request.TextDocument.Uri];
    var cursorPosition = new Cursor {
      Line   = (uint)request.Position.Line + 1,
      Column = (uint)request.Position.Character + 1
    };
    // Get the most specific node at the cursor position.
    var node = ASTUtils.MostSpecificNodeAtCursorPosition(content.AST, cursorPosition);

    if (node is IDocumented documented) {}
    else return null;

    var hoverDebugString =
      $"* Line: {request.Position.Line + 1}\n* Col: {request.Position.Character + 1}\n\n```rad\n{node.Text}\n```";

    // Return the hover information.
    return new Hover {
      Contents = new MarkedStringsOrMarkupContent(
          new MarkupContent {
            Kind = MarkupKind.Markdown,
            Value = documented.Documentation.Markdown
          }
        )
      // Range = new Range(node.Line - 1, node.Column - 1, node.EndLine - 1, node.EndColumn - 1)
    };
  }


  protected override HoverRegistrationOptions CreateRegistrationOptions(
    HoverCapability capability,
    ClientCapabilities clientCapabilities
  ) {
    return new HoverRegistrationOptions {
      DocumentSelector = new DocumentSelector(
          DocumentFilter.ForLanguage("rad")
        )
    };
  }
}
