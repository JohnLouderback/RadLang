using Microsoft.VisualStudio.LanguageServer.Protocol;
using RadLanguageServerV2.Utils;
using RadTypeChecker;

namespace RadLanguageServerV2.Services;

/// <summary>
///   The DiagnosticsService class is responsible for publishing diagnostics for a given document.
/// </summary>
public class DiagnosticsService {
  private readonly LanguageServerService languageServerService;
  private readonly DocumentManagerService documentManagerService;


  /// <param name="languageServerService"> The ILanguageServerFacade to use for publishing diagnostics. </param>
  /// <param name="documentManagerService"> The DocumentManagerService to use for retrieving documents. </param>
  public DiagnosticsService(
    LanguageServerService languageServerService,
    DocumentManagerService documentManagerService
  ) {
    this.languageServerService  = languageServerService;
    this.documentManagerService = documentManagerService;
  }


  /// <summary>
  ///   Publishes the diagnostics for a given document.
  /// </summary>
  /// <param name="forDocument"> The document to run the diagnostics for. </param>
  public void PublishDiagnostics(Uri forDocumentURI) {
    var document     = documentManagerService.Documents[forDocumentURI];
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
        .ToArray();

    languageServerService.PublishDiagnostics(
        new PublishDiagnosticParams {
          Diagnostics = lspDiagnostics,
          Uri         = forDocumentURI
        }
      );
  }
}
