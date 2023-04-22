using System.Diagnostics;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Newtonsoft.Json.Linq;
using RadLanguageServerV2.Constructs;
using RadLanguageServerV2.LanguageServerEx;
using RadLanguageServerV2.LanguageServerEx.Models.InlayHint;
using RadLanguageServerV2.LanguageServerEx.Options;
using RadLanguageServerV2.LanguageServerEx.Options.InlayHints;
using RadLanguageServerV2.LanguageServerEx.Params.InlayHint;
using RadUtils;
using StreamJsonRpc;

namespace RadLanguageServerV2;

/// <summary>
///   The LanguageRPCServer is the actual JSON RPC server which handles incoming requests, delegates
///   them to the broader language server, and finally returns the appropriate response.
/// </summary>
public class LanguageRPCServer {
  private readonly LanguageServer server;

  private readonly string[] serverCommitCharacterArray =
    { " ", "[", "]", "(", ")", ";", "." };

  private readonly string[] itemCommitCharacterArray =
    { " ", "[", "]", "(", ")", ";", "-" };

  private readonly TraceSource traceSource;

  private readonly JsonRpc rpc;
  private readonly HeaderDelimitedMessageHandler messageHandler;

  private int version = 1;
  private int completionItemsNumberRoot;

  private int callCounter;

  public bool SuggestionMode { get; set; } = false;

  public bool IsIncomplete { get; set; } = false;

  public bool CompletionServerError { get; set; } = false;

  public bool ServerCommitCharacters { get; internal set; } = true;
  public bool ItemCommitCharacters { get; internal set; } = false;

  public event EventHandler OnInitializeCompletion;

  public event EventHandler OnInitialized;


  public LanguageRPCServer(
    LanguageServer server,
    TraceSource traceSource,
    Stream output,
    Stream input
  ) {
    this.server      = server;
    this.traceSource = traceSource;
    messageHandler   = new HeaderDelimitedMessageHandler(output, input);
    rpc              = new JsonRpc(messageHandler, this);
    rpc.StartListening();
    //rpc = JsonRpc.Attach(FullDuplexStream.Splice(input, output), 0);
  }


  [JsonRpcMethod(Methods.ExitName)]
  public void Exit() {
    traceSource.TraceEvent(
        TraceEventType.Information,
        0,
        $"{Methods.ExitName} Received Exit notification"
      );
    //server.Exit();
  }


  // [JsonRpcMethod(Methods.TextDocumentCodeActionName)]
  // public object GetCodeActions(JToken arg) {
  //   traceSource.TraceEvent(TraceEventType.Information, 0, $"Received: {arg}");
  //   var parameter = arg.ToObject<CodeActionParams>();
  //   var result    = server.GetCodeActions(parameter);
  //   traceSource.TraceEvent(
  //       TraceEventType.Information,
  //       0,
  //       $"Sent: {JToken.FromObject(result)}"
  //     );
  //   return result;
  // }

  // [JsonRpcMethod(
  //     Methods.TextDocumentDocumentHighlightName,
  //     UseSingleObjectParameterDeserialization = true
  //   )]
  // public DocumentHighlight[] GetDocumentHighlights(
  //   DocumentHighlightParams arg,
  //   CancellationToken token
  // ) {
  //   traceSource.TraceEvent(
  //       TraceEventType.Information,
  //       0,
  //       $"Received: {JToken.FromObject(arg)}"
  //     );
  //   var result = server.GetDocumentHighlights(arg.PartialResultToken, arg.Position, token);
  //   traceSource.TraceEvent(
  //       TraceEventType.Information,
  //       0,
  //       $"Sent: {JToken.FromObject(result)}"
  //     );
  //   return result;
  // }


  [JsonRpcMethod(Methods.TextDocumentDocumentSymbolName)]
  public async Task<DocumentSymbol[]> GetDocumentSymbols(JToken arg) {
    traceSource.TraceEvent(
        TraceEventType.Information,
        0,
        $"{Methods.TextDocumentDocumentSymbolName} Received: {arg}"
      );
    var parameter = arg.ToObject<DocumentSymbolParams>()!;
    return await server.GetHandler<DocumentSymbolParams, DocumentSymbol[]>(
               Methods.TextDocumentDocumentSymbolName
             )(parameter);
  }


  [JsonRpcMethod(MethodsEx.InlayHintsName)]
  public async Task<InlayHint[]> GetInlayHints(JToken arg) {
    traceSource.TraceEvent(
        TraceEventType.Information,
        0,
        $"{MethodsEx.InlayHintsName} Received: {arg}"
      );
    var parameter = arg.ToObject<InlayHintParams>()!;
    return await server.GetHandler<InlayHintParams, InlayHint[]>(
               MethodsEx.InlayHintsName
             )(parameter);
  }


  [JsonRpcMethod(Methods.TextDocumentSemanticTokensFullName)]
  public async Task<SemanticTokens> GetTextDocumentSemanticTokens(JToken arg) {
    traceSource.TraceEvent(
        TraceEventType.Information,
        0,
        $"{Methods.TextDocumentSemanticTokensFullName} Received: {arg}"
      );
    var parameter = arg.ToObject<SemanticTokensParams>()!;
    Debug.WriteLine(
        $"Document Open: {parameter.TextDocument.Uri.AbsolutePath}"
      );
    return await server.GetHandler<SemanticTokensParams, SemanticTokens>(
               Methods.TextDocumentSemanticTokensFullName
             )(parameter);
  }


  // [JsonRpcMethod(Methods.TextDocumentFoldingRangeName)]
  // public object GetFoldingRanges(JToken arg) {
  //   traceSource.TraceEvent(TraceEventType.Information, 0, $"Received: {arg}");
  //   return server.GetFoldingRanges();
  // }

  // [JsonRpcMethod(Methods.CodeActionResolveName)]
  // public object GetResolvedCodeAction(JToken arg) {
  //   traceSource.TraceEvent(TraceEventType.Information, 0, $"Received: {arg}");
  //   var parameter = arg.ToObject<CodeAction>();
  //   var result    = server.GetResolvedCodeAction(parameter);
  //   traceSource.TraceEvent(
  //       TraceEventType.Information,
  //       0,
  //       $"Sent: {JToken.FromObject(result)}"
  //     );
  //   return result;
  // }

  // public string GetText() {
  //   return string.IsNullOrWhiteSpace(server.CustomText)
  //            ? "custom text from language server target"
  //            : server.CustomText;
  // }

  // public string GetWordAtPosition(string fullText, Position position) {
  //   var textLines = fullText.Split(
  //       new[] { Environment.NewLine },
  //       StringSplitOptions.None
  //     );
  //   var textAtSpecifiedLine = textLines[position.Line];
  //
  //   var currentWord = string.Empty;
  //   for (var i = position.Character; i < textAtSpecifiedLine.Length; i++) {
  //     if (textAtSpecifiedLine[i] == ' ') {
  //       break;
  //     }
  //
  //     currentWord += textAtSpecifiedLine[i];
  //   }
  //
  //   for (var i = position.Character - 1; i > 0; i--) {
  //     if (textAtSpecifiedLine[i] == ' ') {
  //       break;
  //     }
  //
  //     currentWord = textAtSpecifiedLine[i] + currentWord;
  //   }
  //
  //   return currentWord;
  // }

  // public Range[] GetWordRangesInText(string fullText, string word) {
  //   List<Range> ranges = new();
  //   var textLines = fullText.Split(
  //       new[] { Environment.NewLine },
  //       StringSplitOptions.None
  //     );
  //   for (var i = 0; i < textLines.Length; i++) {
  //     foreach (Match match in Regex.Matches(textLines[i], word)) {
  //       ranges.Add(
  //           new Range {
  //             Start = new Position(i, match.Index),
  //             End   = new Position(i, match.Index + match.Length)
  //           }
  //         );
  //     }
  //   }
  //
  //   return ranges.ToArray();
  // }


  [JsonRpcMethod(Methods.InitializeName)]
  public object Initialize(JToken arg) {
    traceSource.TraceEvent(
        TraceEventType.Information,
        0,
        $"{Methods.InitializeName} Received: {arg}"
      );

    var capabilities = new ServerCapabilitiesEx {
      TextDocumentSync = new TextDocumentSyncOptions {
        OpenClose = true,
        Change    = TextDocumentSyncKind.Full
      },

      CompletionProvider = new CompletionOptions {
        ResolveProvider     = false,
        TriggerCharacters   = new[] { ",", ".", "@" },
        AllCommitCharacters = serverCommitCharacterArray
      },

      SignatureHelpProvider = new SignatureHelpOptions {
        TriggerCharacters   = new[] { "(", "," },
        RetriggerCharacters = new[] { ")" }
      },

      RenameProvider            = true,
      FoldingRangeProvider      = new FoldingRangeOptions(),
      ReferencesProvider        = true,
      DocumentHighlightProvider = true,
      DocumentSymbolProvider    = true,
      CodeActionProvider        = new CodeActionOptions { ResolveProvider = true },
      HoverProvider             = true,
      SemanticTokensOptions = new SemanticTokensProviderOptions {
        Range = true,
        Legend = new SemanticTokensLegendOptions {
          TokenTypes =
            // Gets all of the token types from the `SemanticTokenType` enum and converts them to
            // camelCase strings.
            typeof(SemanticTokenType).GetEnumNames()
              .Select(token => token.PascalCaseToCamelCase())
              .ToArray(),
          TokenModifiers =
            // Gets all of the token types from the `SemanticTokenModifier` enum and converts them to
            // camelCase strings.
            typeof(SemanticTokenModifier).GetEnumNames()
              .Select(token => token.PascalCaseToCamelCase())
              .ToArray()
        },
        Full = new SemanticTokensFullOptions {
          Delta = true
        }
      },
      InlayHintOptions = new InlayHintOptions {
        ResolveProvider = false
      }
    };

    var result = new InitializeResult {
      Capabilities = capabilities
    };

    OnInitializeCompletion?.Invoke(this, new EventArgs());

    traceSource.TraceEvent(
        TraceEventType.Information,
        0,
        $"Sent: {JToken.FromObject(result)}"
      );

    return result;
  }


  [JsonRpcMethod(Methods.InitializedName)]
  public void Initialized(JToken arg) {
    traceSource.TraceEvent(
        TraceEventType.Information,
        0,
        $"{Methods.InitializedName} Received: {arg}"
      );
    OnInitialized?.Invoke(this, EventArgs.Empty);
  }


  [JsonRpcMethod(Methods.WorkspaceDidChangeConfigurationName)]
  public void OnDidChangeConfiguration(JToken arg) {
    traceSource.TraceEvent(
        TraceEventType.Information,
        0,
        $"{Methods.WorkspaceDidChangeConfigurationName} Received: {arg}"
      );
    var parameter = arg.ToObject<DidChangeConfigurationParams>();
    //server.SendSettings(parameter);
  }


  [JsonRpcMethod(Methods.TextDocumentHoverName)]
  public async Task<Hover> OnHover(JToken arg) {
    traceSource.TraceEvent(
        TraceEventType.Information,
        0,
        $"{Methods.TextDocumentHoverName} Received: {arg}"
      );
    var parameter = arg.ToObject<TextDocumentPositionParams>()!;
    var result =
      await server.GetHandler<TextDocumentPositionParams, Hover?>(Methods.TextDocumentHoverName)(
          parameter
        );

    traceSource.TraceEvent(
        TraceEventType.Information,
        0,
        $"Sent: {(result is null ? "null" : JToken.FromObject(result))}"
      );

    return result;
  }


  [JsonRpcMethod(Methods.TextDocumentDidChangeName)]
  public async Task OnTextDocumentChanged(JToken arg) {
    traceSource.TraceEvent(
        TraceEventType.Information,
        0,
        $"{Methods.TextDocumentDidChangeName} Received: {arg}"
      );
    var parameter = arg.ToObject<DidChangeTextDocumentParams>()!;

    await server.GetHandler<DidChangeTextDocumentParams>(Methods.TextDocumentDidChangeName)(
        parameter
      );

    Debug.WriteLine(
        $"Document Change: {parameter.TextDocument.Uri.AbsolutePath}"
      );
    /*server.UpdateServerSideTextDocument(
        parameter.ContentChanges[0].Text,
        parameter.TextDocument.Version
      );*/
    //server.SendDiagnostics(parameter.TextDocument.Uri);
  }


  // [JsonRpcMethod(Methods.TextDocumentDidCloseName)]
  // public void OnTextDocumentClosed(JToken arg) {
  //   traceSource.TraceEvent(TraceEventType.Information, 0, $"Received: {arg}");
  //   var parameter = arg.ToObject<DidCloseTextDocumentParams>();
  //   Debug.WriteLine(
  //       $"Document Close: {parameter.TextDocument.Uri.AbsolutePath}"
  //     );
  //   server.OnTextDocumentClosed(parameter);
  // }

  // [JsonRpcMethod(Methods.TextDocumentCompletionName)]
  // public CompletionList OnTextDocumentCompletion(JToken arg) {
  //   traceSource.TraceEvent(TraceEventType.Information, 0, $"Received: {arg}");
  //   var parameter = arg.ToObject<CompletionParams>();
  //   var items     = new List<CompletionItem>();
  //
  //   server.LastCompletionRequest = arg.ToString();
  //   var allKinds = Enum.GetValues(typeof(CompletionItemKind)) as CompletionItemKind[];
  //   var itemName = IsIncomplete ? "Incomplete" : "Item";
  //   for (var i = 0; i < 10; i++) {
  //     var item = new CompletionItem();
  //     item.Label         = $"{itemName} {i + completionItemsNumberRoot}";
  //     item.InsertText    = $"{itemName}{i + completionItemsNumberRoot}";
  //     item.SortText      = item.Label;
  //     item.Kind          = allKinds[(completionItemsNumberRoot + i) % allKinds.Length];
  //     item.Detail        = $"Detail for {itemName} {i + completionItemsNumberRoot}";
  //     item.Documentation = $"Documentation for {itemName} {i + completionItemsNumberRoot}";
  //     if (ItemCommitCharacters) {
  //       item.CommitCharacters = itemCommitCharacterArray;
  //     }
  //     else {
  //       item.CommitCharacters = null;
  //     }
  //
  //     items.Add(item);
  //   }
  //
  //   completionItemsNumberRoot += 10;
  //
  //   // Items to test sorting, when SortText is equal items should be ordered by label
  //   // So the following 3 items will be sorted: B, A, C.
  //   // B comes first being SortText the first sorting criteria
  //   // Then A and C have same SortText, so they are sorted by Label coming A before C
  //   var cItem = new CompletionItem();
  //   cItem.Label      = "C";
  //   cItem.InsertText = "C Kind";
  //   cItem.SortText   = "2";
  //   cItem.Kind       = 0;
  //   items.Add(cItem);
  //
  //   var bItem = new CompletionItem();
  //   bItem.Label      = "B";
  //   bItem.InsertText = "B Kind";
  //   bItem.SortText   = "1";
  //   bItem.Kind       = 0;
  //   items.Add(bItem);
  //
  //   var aItem = new CompletionItem();
  //   aItem.Label      = "A";
  //   aItem.InsertText = "A Kind";
  //   aItem.SortText   = "2";
  //   aItem.Kind       = 0;
  //   items.Add(aItem);
  //
  //   var invalidItem = new CompletionItem();
  //   invalidItem.Label      = "Invalid";
  //   invalidItem.InsertText = "Invalid Kind";
  //   invalidItem.SortText   = "Invalid";
  //   invalidItem.Kind       = 0;
  //   items.Add(invalidItem);
  //
  //   var fileNames = new[] {
  //     "sample.txt", "myHeader.h", "src/Feature/MyClass.cs", "../resources/img/sample.png",
  //     "http://contoso.com/awesome/Index.razor",
  //     "http://schemas.microsoft.com/winfx/2006/xaml/file.xml"
  //   };
  //   for (var i = 0; i < fileNames.Length; i++) {
  //     var item = new CompletionItem();
  //     item.Label      = fileNames[i];
  //     item.InsertText = fileNames[i];
  //     item.SortText   = fileNames[i];
  //     item.Kind       = CompletionItemKind.File;
  //     item.Documentation =
  //       $"Verifies whether IVsImageService provided correct icon for {fileNames[i]}";
  //     item.CommitCharacters = new[] { "." };
  //     items.Add(item);
  //   }
  //
  //   var list = new CompletionList {
  //     IsIncomplete = IsIncomplete ||
  //                    parameter.Context.TriggerCharacter == "@" ||
  //                    (parameter.Context.TriggerKind ==
  //                     CompletionTriggerKind.TriggerForIncompleteCompletions &&
  //                     completionItemsNumberRoot % 50 != 0),
  //     Items = items.ToArray()
  //   };
  //
  //   server.IsIncomplete = false;
  //
  //   if (CompletionServerError) {
  //     throw new InvalidOperationException("Simulated server error.");
  //   }
  //
  //   traceSource.TraceEvent(
  //       TraceEventType.Information,
  //       0,
  //       $"Sent: {JToken.FromObject(list)}"
  //     );
  //
  //   return list;
  // }

  // [JsonRpcMethod(
  //     Methods.TextDocumentReferencesName,
  //     UseSingleObjectParameterDeserialization = true
  //   )]
  // public object[] OnTextDocumentFindReferences(
  //   ReferenceParams parameter,
  //   CancellationToken token
  // ) {
  //   traceSource.TraceEvent(
  //       TraceEventType.Information,
  //       0,
  //       $"Received: {JToken.FromObject(parameter)}"
  //     );
  //   var result = server.SendReferences(parameter, returnLocationsOnly: true, token: token);
  //   traceSource.TraceEvent(
  //       TraceEventType.Information,
  //       0,
  //       $"Sent: {JToken.FromObject(result)}"
  //     );
  //   return result;
  // }


  [JsonRpcMethod(Methods.TextDocumentDidOpenName)]
  public async Task OnTextDocumentOpened(JToken arg) {
    traceSource.TraceEvent(
        TraceEventType.Information,
        0,
        $"{Methods.TextDocumentDidOpenName} Received: {arg}"
      );
    var parameter = arg.ToObject<DidOpenTextDocumentParams>()!;
    Debug.WriteLine(
        $"Document Open: {parameter.TextDocument.Uri.AbsolutePath}"
      );
    await server.GetHandler<DidOpenTextDocumentParams>(Methods.TextDocumentDidOpenName)(parameter);
  }


  public Task SendMethodNotificationAsync<TIn>(LspNotification<TIn> method, TIn param) {
    return rpc.NotifyWithParameterObjectAsync(method.Name, param);
  }


  public Task<TOut> SendMethodRequestAsync<TIn, TOut>(LspRequest<TIn, TOut> method, TIn param) {
    return rpc.InvokeWithParameterObjectAsync<TOut>(method.Name, param);
  }


  // [JsonRpcMethod(Methods.TextDocumentSignatureHelpName)]
  // public SignatureHelp OnTextDocumentSignatureHelp(JToken arg) {
  //   traceSource.TraceEvent(TraceEventType.Information, 0, $"Received: {arg}");
  //
  //   SignatureHelp retVal = null;
  //   if (callCounter < 4) {
  //     retVal = new SignatureHelp {
  //       ActiveParameter = callCounter % 2,
  //       ActiveSignature = callCounter / 2,
  //       Signatures = new[] {
  //         new SignatureInformation {
  //           Label = "foo(param1, param2)",
  //           Parameters = new[] {
  //             new() {
  //               Label = "param1"
  //             },
  //             new ParameterInformation {
  //               Label = "param2"
  //             }
  //           }
  //         },
  //         new SignatureInformation {
  //           Label = "foo(param1, param2, param3)",
  //           Parameters = new[] {
  //             new ParameterInformation {
  //               Label = "param1"
  //             },
  //             new ParameterInformation {
  //               Label = "param2"
  //             },
  //             new ParameterInformation {
  //               Label = "param3"
  //             }
  //           }
  //         }
  //       }
  //     };
  //   }
  //
  //   callCounter = (callCounter + 1) % 5;
  //
  //   traceSource.TraceEvent(
  //       TraceEventType.Information,
  //       0,
  //       $"Sent: {JToken.FromObject(retVal)}"
  //     );
  //
  //   return retVal;
  // }

  // [JsonRpcMethod(Methods.TextDocumentRenameName)]
  // public WorkspaceEdit Rename(JToken arg) {
  //   traceSource.TraceEvent(TraceEventType.Information, 0, $"Received: {arg}");
  //   var renameParams    = arg.ToObject<RenameParams>();
  //   var fullText        = File.ReadAllText(renameParams.TextDocument.Uri.LocalPath);
  //   var wordToReplace   = GetWordAtPosition(fullText, renameParams.Position);
  //   var placesToReplace = GetWordRangesInText(fullText, wordToReplace);
  //
  //   var result = new WorkspaceEdit {
  //     DocumentChanges = new[] {
  //       new() {
  //         TextDocument = new OptionalVersionedTextDocumentIdentifier {
  //           Uri     = renameParams.TextDocument.Uri,
  //           Version = ++version
  //         },
  //         Edits = placesToReplace.Select(
  //               range =>
  //                 new TextEdit {
  //                   NewText = renameParams.NewName,
  //                   Range   = range
  //                 }
  //             )
  //           .ToArray()
  //       }
  //     }
  //   };
  //
  //   traceSource.TraceEvent(
  //       TraceEventType.Information,
  //       0,
  //       $"Sent: {JToken.FromObject(result)}"
  //     );
  //   return result;
  // }


  [JsonRpcMethod(Methods.ShutdownName)]
  public object Shutdown() {
    traceSource.TraceEvent(
        TraceEventType.Information,
        0,
        $"{Methods.ShutdownName} Received Shutdown notification"
      );
    return null;
  }
}
