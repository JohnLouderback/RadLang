using RadLanguageServerV2.ASTVisitors;
using RadLanguageServerV2.Exceptions;
using RadLanguageServerV2.LanguageServerEx.Models.InlayHint;
using RadLanguageServerV2.LanguageServerEx.Params.InlayHint;
using RadLanguageServerV2.Services;

namespace RadLanguageServerV2.Handlers;

public class GetInlayHintsHandler : IRequestHandler<InlayHintParams, InlayHint[]> {
  private readonly DocumentManagerService documentManagerService;


  public GetInlayHintsHandler(DocumentManagerService documentManagerService) {
    this.documentManagerService = documentManagerService;
  }


  public async Task<InlayHint[]> Handler(InlayHintParams args) {
    var content = documentManagerService.Documents[args.TextDocument.Uri] ??
                  throw new TextDocumentNotFoundException(args.TextDocument.Uri);

    var inlayHintsVisitor = new InlayHintASTVisitor();
    inlayHintsVisitor.Visit(content.AST!);
    return inlayHintsVisitor.InlayHints.ToArray();
  }
}
