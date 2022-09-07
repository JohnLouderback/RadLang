using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using RadLanguageServer.ASTVisitors;
using RadLanguageServer.Services;

namespace RadLanguageServer.Handlers;

public class SemanticTokensHandler : SemanticTokensHandlerBase {
  private readonly ILogger logger;
  private readonly DocumentManagerService documentManagerService;


  public SemanticTokensHandler(
    ILogger<SemanticTokensHandler> logger,
    DocumentManagerService documentManagerService
  ) {
    this.logger                 = logger;
    this.documentManagerService = documentManagerService;
  }


  public override async Task<SemanticTokens?> Handle(
    SemanticTokensParams request,
    CancellationToken cancellationToken
  ) {
    var result = await base.Handle(request, cancellationToken).ConfigureAwait(false);
    return result;
  }


  public override async Task<SemanticTokens?> Handle(
    SemanticTokensRangeParams request,
    CancellationToken cancellationToken
  ) {
    var result = await base.Handle(request, cancellationToken).ConfigureAwait(false);
    return result;
  }


  public override async Task<SemanticTokensFullOrDelta?> Handle(
    SemanticTokensDeltaParams request,
    CancellationToken cancellationToken
  ) {
    var result = await base.Handle(request, cancellationToken).ConfigureAwait(false);
    return result;
  }


  protected override SemanticTokensRegistrationOptions CreateRegistrationOptions(
    SemanticTokensCapability capability,
    ClientCapabilities clientCapabilities
  ) {
    return new SemanticTokensRegistrationOptions {
      DocumentSelector = DocumentSelector.ForLanguage("rad"),
      Legend = new SemanticTokensLegend {
        TokenModifiers = capability.TokenModifiers,
        TokenTypes     = capability.TokenTypes
      },
      Full = new SemanticTokensCapabilityRequestFull {
        Delta = true
      },
      Range = true
    };
  }


  protected override Task<SemanticTokensDocument>
    GetSemanticTokensDocument(
      ITextDocumentIdentifierParams @params,
      CancellationToken cancellationToken
    ) {
    return Task.FromResult(new SemanticTokensDocument(RegistrationOptions.Legend));
  }


  /// <summary>
  ///   Tokenizes the given text document.
  /// </summary>
  /// <param name="builder">
  ///   The <see cref="SemanticTokensBuilder" /> to use for building the tokens.
  /// </param>
  /// <param name="identifier">
  ///   The <see cref="ITextDocumentIdentifierParams" /> for the text document to tokenize.
  /// </param>
  /// <param name="cancellationToken">
  ///   A <see cref="CancellationToken" /> that can be used to cancel the operation.
  /// </param>
  /// <returns> A <see cref="Task" /> that represents the asynchronous operation. </returns>
  protected override async Task Tokenize(
    SemanticTokensBuilder builder,
    ITextDocumentIdentifierParams identifier,
    CancellationToken cancellationToken
  ) {
    // Get the stored document and visit its AST node to generate the tokens.
    var content           = documentManagerService.Documents[identifier.TextDocument.Uri];
    var semanticTokenizer = new SemanticTokenASTVisitor(builder);
    semanticTokenizer.Visit(content.AST);
    semanticTokenizer.BuildTokens();
    Console.Write("");
  }
}
