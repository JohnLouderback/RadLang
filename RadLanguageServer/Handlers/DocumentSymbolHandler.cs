using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using RadLanguageServer.ASTVisitors;
using RadLanguageServer.Services;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace RadLanguageServer.Handlers;

internal class DocumentSymbolHandler : IDocumentSymbolHandler {
  private readonly DocumentManagerService documentManagerService;


  public DocumentSymbolHandler(DocumentManagerService documentManagerService) {
    this.documentManagerService = documentManagerService;
  }


  public DocumentSymbolRegistrationOptions GetRegistrationOptions(
    DocumentSymbolCapability capability,
    ClientCapabilities clientCapabilities
  ) {
    return new DocumentSymbolRegistrationOptions {
      DocumentSelector = DocumentSelector.ForLanguage("rad")
    };
  }


  public async Task<SymbolInformationOrDocumentSymbolContainer> Handle(
    DocumentSymbolParams request,
    CancellationToken cancellationToken
  ) {
    // Get the stored document and visit its AST node to generate the tokens.
    var content = documentManagerService.Documents[request.TextDocument.Uri];
    var documentSymbolVisitor = new DocumentSymbolASTVisitor();
    documentSymbolVisitor.Visit(content.AST);

    // Return the root module symbol.
    var response = documentSymbolVisitor.RootSymbol?.Children?.Select(child => (SymbolInformationOrDocumentSymbol)child);
    return SymbolInformationOrDocumentSymbolContainer.From(response ?? new List<SymbolInformationOrDocumentSymbol>());
  }
}
