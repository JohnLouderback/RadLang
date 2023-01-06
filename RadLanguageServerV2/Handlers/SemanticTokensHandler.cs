using Microsoft.VisualStudio.LanguageServer.Protocol;
using RadLanguageServer.ASTVisitors;
using RadLanguageServerV2.Constructs;
using RadLanguageServerV2.Exceptions;
using RadLanguageServerV2.Services;

namespace RadLanguageServerV2.Handlers;

public class SemanticTokensHandler : IRequestHandler<SemanticTokensParams, SemanticTokens> {
  private readonly DocumentManagerService documentManagerService;


  public SemanticTokensHandler(DocumentManagerService documentManagerService) {
    this.documentManagerService = documentManagerService;
  }


  public async Task<SemanticTokens> Handler(SemanticTokensParams args) {
    var semanticTokensDocument = new SemanticTokensDocument();

    var content = documentManagerService.Documents[args.TextDocument.Uri] ??
                  throw new TextDocumentNotFoundException(args.TextDocument.Uri);

    var semanticTokenizer = new SemanticTokenASTVisitor(semanticTokensDocument);
    semanticTokenizer.Visit(content.AST!);
    semanticTokenizer.BuildTokens();

    return new SemanticTokens {
      Data = semanticTokensDocument.Data.ToArray()
    };
  }
}
