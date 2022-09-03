using System.Collections.Immutable;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using RadLanguageServer.Utils;
using RadTypeChecker;

namespace RadLanguageServer;

public class DiagnosticsService {
  private readonly ILanguageServerFacade facade;
  private readonly DocumentManagerService documentManagerService;


  public DiagnosticsService(
    ILanguageServerFacade facade,
    DocumentManagerService documentManagerService
  ) {
    this.facade                 = facade;
    this.documentManagerService = documentManagerService;
  }


  /// <summary>
  ///   Publishes the diagnostics for a given document.
  /// </summary>
  /// <param name="forDocument"> The document to run the diagnostics for. </param>
  public void PublishDiagnostics(TextDocumentIdentifier forDocument) {
    // Get the AST for the document
    var ast = documentManagerService.Documents[forDocument.Uri].AST;

    // Statically type-check the AST.
    var typeErrors = new TypeChecker().CheckTypes(ast);

    // Map an immutable array of LSP Diagnostics from the Rad Type Checker Diagnostics.
    var lspDiagnostics =
      typeErrors.Select(typeError => typeError.ToLSPDiagnostic()).ToImmutableArray();

    facade.TextDocument.PublishDiagnostics(
        new PublishDiagnosticsParams {
          Diagnostics = new Container<Diagnostic>(lspDiagnostics.ToArray()),
          Uri         = forDocument.Uri
        }
      );
  }
}
