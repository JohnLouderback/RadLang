using Microsoft.VisualStudio.LanguageServer.Protocol;
using RadLanguageServerV2.Constructs;
using RadLanguageServerV2.Services;

namespace RadLanguageServerV2.Handlers;

public class DidChangeTextDocumentHandler : IVoidRequestHandler<DidChangeTextDocumentParams> {
  private readonly DocumentManagerService documentManagerService;


  public DidChangeTextDocumentHandler(DocumentManagerService documentManagerService) {
    this.documentManagerService = documentManagerService;
  }


  public async Task Handler(DidChangeTextDocumentParams args) {
    var contentChanges = args.ContentChanges;
    var textDocument   = args.TextDocument;

    // If the content change is the full document.
    if (contentChanges.Length == 1 &&
        contentChanges[0].Range == null) {
      var change = contentChanges[0].Text;

      // Check if the the document already exists.
      if (documentManagerService.Documents.TryGetValue(
              textDocument.Uri,
              out var document
            )) {
        // If it does, update it.
        document.Update(change);
      }

      // Otherwise, create a new document.
      else {
        documentManagerService.Documents.Add(
            textDocument.Uri,
            new DocumentContent(change)
          );
      }
    }
  }
}
