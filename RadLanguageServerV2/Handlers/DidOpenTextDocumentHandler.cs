using Microsoft.VisualStudio.LanguageServer.Protocol;
using RadLanguageServerV2.Constructs;
using RadLanguageServerV2.Services;

namespace RadLanguageServerV2.Handlers;

public class DidOpenTextDocumentHandler : IVoidRequestHandler<DidOpenTextDocumentParams> {
  private readonly DocumentManagerService documentManagerService;


  public DidOpenTextDocumentHandler(DocumentManagerService documentManagerService) {
    this.documentManagerService = documentManagerService;
  }


  public async Task Handler(DidOpenTextDocumentParams args) {
    var document = args.TextDocument;
    documentManagerService.Documents.Add(
        document.Uri,
        new DocumentContent(document.Text)
      );
  }
}
