using Microsoft.VisualStudio.LanguageServer.Protocol;
using RadLanguageServerV2.Constructs;
using RadLanguageServerV2.Services;

namespace RadLanguageServerV2.Handlers;

public class DidOpenTextDocumentHandler : IVoidRequestHandler<DidOpenTextDocumentParams> {
  private readonly DocumentManagerService documentManagerService;
  private readonly DiagnosticsService diagnosticsService;


  public DidOpenTextDocumentHandler(
    DocumentManagerService documentManagerService,
    DiagnosticsService diagnosticsService
  ) {
    this.documentManagerService = documentManagerService;
    this.diagnosticsService     = diagnosticsService;
  }


  public async Task Handler(DidOpenTextDocumentParams args) {
    var document = args.TextDocument;
    documentManagerService.Documents.Add(
        document.Uri,
        new DocumentContent(document.Text)
      );

    await Task.Yield();
    diagnosticsService.PublishDiagnostics(document.Uri);
  }
}
