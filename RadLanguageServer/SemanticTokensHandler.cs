using Antlr4.Runtime;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using RadLexer;
using RadParser;

namespace RadLanguageServer;

public class SemanticTokensHandler : SemanticTokensHandlerBase {
  private readonly ILogger logger;
  private readonly DocumentManager documentManager;


  public SemanticTokensHandler(
    ILogger<SemanticTokensHandler> logger,
    DocumentManager documentManager
  ) {
    this.logger          = logger;
    this.documentManager = documentManager;
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
    // you would normally get this from a common source that is managed by current open editor,
    // current active editor, etc.
    var content = documentManager.Documents[identifier.TextDocument.Uri];

    // Get the input stream from the file content provided.
    var inputStream = new AntlrInputStream(
        content.Text
      );

    // Create a lexer based on the input stream.
    var lexer = new RadLexer.RadLexer(inputStream);

    // Tokenize the file and get the token stream from the lexer.
    var tokenStream = new CommonTokenStream(lexer);

    // Create a token parser.
    var parser = new Rad(tokenStream);

    // Parse the tokens to generate the "Concrete Syntax Tree".
    var cst = parser.startRule();

    // Get the AST.
    var ast = new ASTGenerator().GenerateASTFromCST(cst);

    var semanticTokenizer = new SemanticTokenASTVisitor(builder);
    semanticTokenizer.Visit(ast);
    semanticTokenizer.BuildTokens();
    Console.Write("");
  }
}
