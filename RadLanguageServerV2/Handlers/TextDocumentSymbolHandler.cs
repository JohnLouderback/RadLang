using Microsoft.VisualStudio.LanguageServer.Protocol;
using RadLanguageServer.ASTVisitors;
using RadLanguageServerV2.Exceptions;
using RadLanguageServerV2.Services;

namespace RadLanguageServerV2.Handlers;

public class TextDocumentSymbolHandler : IRequestHandler<DocumentSymbolParams, DocumentSymbol[]> {
  private readonly DocumentManagerService documentManagerService;


  public TextDocumentSymbolHandler(DocumentManagerService documentManagerService) {
    this.documentManagerService = documentManagerService;
  }


  public async Task<DocumentSymbol[]> Handler(DocumentSymbolParams args) {
    var content = documentManagerService.Documents[args.TextDocument.Uri] ??
                  throw new TextDocumentNotFoundException(args.TextDocument.Uri);

    var documentSymbolVisitor = new DocumentSymbolASTVisitor();
    documentSymbolVisitor.Visit(content.AST!);

    // Return the root module symbol.
    return documentSymbolVisitor.RootSymbol.Children;
  }
}
