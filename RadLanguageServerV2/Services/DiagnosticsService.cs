using Microsoft.VisualStudio.LanguageServer.Protocol;
using RadTypeChecker;

namespace RadLanguageServerV2.Services;

/// <summary>
///   The DiagnosticsService class is responsible for publishing diagnostics for a given document.
/// </summary>
public class DiagnosticsService {
  private readonly ILanguageServerFacade facade;
  private readonly DocumentManagerService documentManagerService;


  /// <inheritdoc />
  /// <param name="facade"> The ILanguageServerFacade to use for publishing diagnostics. </param>
  /// <param name="documentManagerService"> The DocumentManagerService to use for retrieving documents. </param>
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
    var document     = documentManagerService.Documents[forDocument.Uri];
    var syntaxErrors = document.SyntaxErrors;
    // Get the AST for the document
    var ast = document.AST;

    // Statically type-check the AST.
    var typeErrors = new TypeChecker().CheckTypes(ast);

    // Map an immutable array of LSP Diagnostics from the Rad Type Checker Diagnostics.
    var lspDiagnostics =
      // Get type errors.
      typeErrors.Select(typeError => typeError.ToLSPDiagnostic())
        // Combine with any syntax errors.
        .Concat(syntaxErrors.Select(syntaxError => syntaxError.ToLSPDiagnostic()))
        .ToImmutableArray();

    facade.TextDocument.PublishDiagnostics(
        new PublishDiagnosticsParams {
          Diagnostics = new Container<Diagnostic>(lspDiagnostics.ToArray()),
          Uri         = forDocument.Uri
        }
      );
  }
}
