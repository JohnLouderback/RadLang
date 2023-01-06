using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Newtonsoft.Json.Linq;
using StreamJsonRpc;
using Range = System.Range;

namespace RadLanguageServerV2;

public class XLanguageServer : INotify {
  private readonly HeaderDelimitedMessageHandler messageHandler;
  private readonly LanguageServerTarget target;
  private readonly ManualResetEvent disconnectEvent = new(false);
  private readonly Dictionary<VSTextDocumentIdentifier, int> diagnosticsResults;
  private int maxProblems = -1;
  private List<DiagnosticsInfo> diagnostics;
  private TextDocumentItem textDocument;
  private string referenceToFind;
  private int referencesChunkSize;
  private int referencesDelay;
  private int highlightChunkSize;
  private int highlightsDelay;

  private int counter = 100;

  private string lastCompletionRequest = string.Empty;

  public string CustomText { get; set; }

  public bool IsIncomplete {
    get => target.IsIncomplete;
    set {
      if (target.IsIncomplete != value) {
        target.IsIncomplete = value;
        NotifyPropertyChanged(nameof(IsIncomplete));
      }
    }
  }

  public bool CompletionServerError {
    get => target.CompletionServerError;
    set => target.CompletionServerError = value;
  }

  public bool ItemCommitCharacters {
    get => target.ItemCommitCharacters;
    set => target.ItemCommitCharacters = value;
  }

  public string CurrentSettings { get; private set; }

  public JsonRpc Rpc { get; }

  public IEnumerable<FoldingRange> FoldingRanges { get; set; }

  public IEnumerable<VSSymbolInformation> Symbols { get; set; }

  public IEnumerable<VSProjectContext> Contexts { get; set; } = Array.Empty<VSProjectContext>();

  public bool UsePublishModelDiagnostic { get; set; } = true;

  public string LastCompletionRequest {
    get => lastCompletionRequest;
    set {
      lastCompletionRequest = value ?? string.Empty;
      NotifyPropertyChanged(nameof(LastCompletionRequest));
    }
  }

  public event EventHandler Disconnected;

  public event EventHandler OnInitialized;
  public event PropertyChangedEventHandler PropertyChanged;


  public LanguageServer(
    Stream sender,
    Stream reader,
    List<DiagnosticsInfo> initialDiagnostics = null
  ) {
    var jsonRpcTraceSource = LogUtils.CreateTraceSource();
    target           =  new LanguageServerTarget(this, jsonRpcTraceSource);
    messageHandler   =  new HeaderDelimitedMessageHandler(sender, reader);
    Rpc              =  new JsonRpc(messageHandler, target);
    Rpc.Disconnected += OnRpcDisconnected;

    Rpc.ActivityTracingStrategy = new CorrelationManagerTracingStrategy {
      TraceSource = jsonRpcTraceSource
    };
    Rpc.TraceSource = jsonRpcTraceSource;

    ((JsonMessageFormatter)messageHandler.Formatter).JsonSerializer.Converters.Add(
        new VSExtensionConverter<TextDocumentIdentifier, VSTextDocumentIdentifier>()
      );

    Rpc.StartListening();

    diagnostics        = initialDiagnostics;
    diagnosticsResults = new Dictionary<VSTextDocumentIdentifier, int>();

    FoldingRanges = Array.Empty<FoldingRange>();
    Symbols       = Array.Empty<VSSymbolInformation>();

    target.OnInitializeCompletion += OnTargetInitializeCompletion;
    target.OnInitialized          += OnTargetInitialized;
  }


  public void ApplyTextEdit(string text) {
    if (textDocument == null) {
      return;
    }

    TextEdit[] addTextEdit = {
      new() {
        Range = new Microsoft.VisualStudio.LanguageServer.Protocol.Range {
          Start = new Position {
            Line      = 0,
            Character = 0
          },
          End = new Position {
            Line      = 0,
            Character = 0
          }
        },
        NewText = text
      }
    };

    var parameter = new ApplyWorkspaceEditParams {
      Label = "Test Edit",
      Edit = new WorkspaceEdit {
        DocumentChanges = new[] {
          new() {
            TextDocument = new OptionalVersionedTextDocumentIdentifier {
              Uri = textDocument.Uri
            },
            Edits = addTextEdit
          }
        }
      }
    };

    _ = Task.Run(
        async () => {
          var response = await SendMethodRequestAsync(Methods.WorkspaceApplyEdit, parameter);

          if (!response.Applied) {
            Console.WriteLine($"Failed to apply edit: {response.FailureReason}");
          }
        }
      );
  }


  public void Exit() {
    disconnectEvent.Set();

    Disconnected?.Invoke(this, new EventArgs());
  }


  public object[] GetCodeActions(CodeActionParams parameter) {
    #region File Operation actions
    var documentUri             = parameter.TextDocument.Uri;
    var absolutePath            = documentUri.LocalPath.TrimStart('/');
    var documentFilePath        = Path.GetFullPath(absolutePath);
    var documentDirectory       = Path.GetDirectoryName(documentFilePath);
    var documentNameNoExtension = Path.GetFileNameWithoutExtension(documentFilePath);
    var createFilePath          = Path.Combine(documentDirectory, documentNameNoExtension + ".txt");

    var createFileUri = new UriBuilder {
      Path   = createFilePath,
      Host   = string.Empty,
      Scheme = Uri.UriSchemeFile
    }.Uri;

    var createFileAction = new CodeAction {
      Title = "Create <TheCurrentFile>.txt",
      Edit = new WorkspaceEdit {
        DocumentChanges = new SumType<TextDocumentEdit, CreateFile, RenameFile, DeleteFile>[] {
          new CreateFile {
            Uri = createFileUri,
            Options = new CreateFileOptions {
              Overwrite = true
            }
          }
        }
      }
    };

    var renameNewFilePath = Path.Combine(
        documentDirectory,
        documentNameNoExtension + "_Renamed.txt"
      );

    var renameNewFileUri = new UriBuilder {
      Path   = renameNewFilePath,
      Host   = string.Empty,
      Scheme = Uri.UriSchemeFile
    }.Uri;

    var renameFileAction = new CodeAction {
      Title = "Rename <TheCurrentFile>.txt to <TheCurrentFile>_Renamed.txt",
      Edit = new WorkspaceEdit {
        DocumentChanges = new SumType<TextDocumentEdit, CreateFile, RenameFile, DeleteFile>[] {
          new RenameFile {
            OldUri = createFileUri,
            NewUri = renameNewFileUri,
            Options = new RenameFileOptions {
              Overwrite = true
            }
          }
        }
      }
    };
    #endregion

    #region Add Text actions
    TextEdit[] addTextEdit = {
      new() {
        Range = new Range {
          Start = new Position {
            Line      = 0,
            Character = 0
          },
          End = new Position {
            Line      = 0,
            Character = 0
          }
        },
        NewText = "Added text!"
      }
    };

    var addTextAction = new CodeAction {
      Title = "Add Text Action - DocumentChanges property",
      Edit = new WorkspaceEdit {
        DocumentChanges = new[] {
          new() {
            TextDocument = new OptionalVersionedTextDocumentIdentifier {
              Uri = parameter.TextDocument.Uri
            },
            Edits = addTextEdit
          }
        }
      },
      Kind = CodeActionKind.QuickFix
    };

    var changes = new Dictionary<string, TextEdit[]>();
    changes.Add(parameter.TextDocument.Uri.AbsoluteUri, addTextEdit);

    var addTextActionChangesProperty = new CodeAction {
      Title = "Add Text Action - Changes property",
      Edit = new WorkspaceEdit {
        Changes = changes
      },
      Kind = CodeActionKind.QuickFix
    };

    var addUnderscoreAction = new CodeAction {
      Title = "Add _",
      Edit = new WorkspaceEdit {
        DocumentChanges = new[] {
          new TextDocumentEdit {
            TextDocument = new OptionalVersionedTextDocumentIdentifier {
              Uri = parameter.TextDocument.Uri
            },
            Edits = new[] {
              new() {
                Range = new Range {
                  Start = new Position {
                    Line      = 0,
                    Character = 0
                  },
                  End = new Position {
                    Line      = 0,
                    Character = 0
                  }
                },
                NewText = "_"
              }
            }
          }
        }
      },
      Kind = CodeActionKind.QuickFix
    };

    var addTextActionWithError = new CodeAction {
      Title = "Add Text Action - with error diagnostic",
      Edit = new WorkspaceEdit {
        Changes = changes
      },
      Diagnostics = new[] {
        new() {
          Range = new Range {
            Start = new Position {
              Line      = 0,
              Character = 0
            },
            End = new Position {
              Line      = 0,
              Character = 0
            }
          },
          Message  = "Test Error",
          Severity = DiagnosticSeverity.Error
        }
      },
      Kind = CodeActionKind.QuickFix
    };

    var editFilePath = Path.Combine(documentDirectory, documentNameNoExtension + "2.foo");
    var addTextActionToOtherFile = new CodeAction {
      Title = "Add Text Action - Edit on different file",
      Edit = new WorkspaceEdit {
        DocumentChanges = new[] {
          new() {
            TextDocument = new OptionalVersionedTextDocumentIdentifier {
              Uri = new Uri(editFilePath)
            },
            Edits = addTextEdit
          }
        }
      },
      Kind = CodeActionKind.QuickFix
    };
    #endregion

    #region Unresolved actions
    var unresolvedAddText = new CodeAction {
      Title = "Unresolved Add Text Action",
      Data  = addTextAction
    };
    #endregion

    return new[] {
      addTextAction,
      addTextActionChangesProperty,
      addTextActionWithError,
      addTextActionToOtherFile,
      unresolvedAddText,
      addUnderscoreAction,
      createFileAction,
      renameFileAction
    };
  }


  public DocumentHighlight[] GetDocumentHighlights(
    IProgress<DocumentHighlight[]> progress,
    Position position,
    CancellationToken token
  ) {
    if (textDocument == null ||
        progress == null) {
      return Array.Empty<DocumentHighlight>();
    }

    var delay = highlightsDelay;
    var lines = textDocument.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

    // Default to "error" if no word is selected
    var currentHighlightedWord = GetWordAtPosition(position, lines);
    if (string.IsNullOrEmpty(currentHighlightedWord)) {
      currentHighlightedWord = "error";
    }

    highlightChunkSize = Math.Max(highlightChunkSize, 1);

    var highlights = new List<DocumentHighlight>();
    var chunk      = new List<DocumentHighlight>();

    for (var i = 0; i < lines.Length; i++) {
      var line = lines[i];

      for (var j = 0; j < line.Length; j++) {
        var range = GetHighlightRange(line, i, ref j, currentHighlightedWord);

        if (range != null) {
          var highlight = new DocumentHighlight
            { Range = range, Kind = DocumentHighlightKind.Text };
          highlights.Add(highlight);
          chunk.Add(highlight);

          if (chunk.Count == highlightChunkSize) {
            progress.Report(chunk.ToArray());
            Thread.Sleep(delay); // Wait between chunks
            chunk.Clear();
          }
        }

        token.ThrowIfCancellationRequested();
      }
    }

    // Report last chunk if it has elements since it didn't reached the specified size
    if (chunk.Count() > 0) {
      progress.Report(chunk.ToArray());
    }

    return highlights.ToArray();
  }


  public VSSymbolInformation[] GetDocumentSymbols() {
    return Symbols.ToArray();
  }


  public FoldingRange[] GetFoldingRanges() {
    return FoldingRanges.ToArray();
  }


  public VSProjectContextList GetProjectContexts() {
    var result = new VSProjectContextList();
    result.ProjectContexts = Contexts.ToArray();
    result.DefaultIndex    = 0;

    return result;
  }


  public CodeAction GetResolvedCodeAction(CodeAction parameter) {
    var token              = JObject.FromObject(parameter.Data);
    var resolvedCodeAction = token.ToObject<CodeAction>();

    return resolvedCodeAction;
  }


  public void LogMessage(object arg) {
    LogMessage(arg, MessageType.Info);
  }


  public void LogMessage(object arg, MessageType messageType) {
    LogMessage(arg, "testing " + counter++, messageType);
  }


  public void LogMessage(object arg, string message, MessageType messageType) {
    var parameter = new LogMessageParams {
      Message     = message,
      MessageType = messageType
    };
    _ = SendMethodNotificationAsync(Methods.WindowLogMessage, parameter);
  }


  public void OnTextDocumentClosed(DidCloseTextDocumentParams messageParams) {
    textDocument = null;
  }


  public void OnTextDocumentOpened(DidOpenTextDocumentParams messageParams) {
    textDocument = messageParams.TextDocument;

    SendDiagnostics();
  }


  public void SendDiagnostics(List<DiagnosticsInfo> sentDiagnostics, bool pushDiagnostics) {
    if (diagnostics == null) {
      diagnostics = new List<DiagnosticsInfo>();
    }

    diagnostics = sentDiagnostics;

    if (pushDiagnostics) {
      SendDiagnostics(sentDiagnostics);
    }
  }


  public void SendDiagnostics() {
    SendDiagnostics(diagnostics);
  }


  public void SendDiagnostics(List<DiagnosticsInfo> sentDiagnostics) {
    if (textDocument == null ||
        sentDiagnostics == null ||
        !UsePublishModelDiagnostic) {
      return;
    }

    var lines = textDocument.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

    var diagnostics = new List<Diagnostic>();
    for (var i = 0; i < lines.Length; i++) {
      var line = lines[i];

      var j = 0;
      while (j < line.Length) {
        Diagnostic diagnostic = null;
        foreach (var tag in sentDiagnostics) {
          diagnostic = GetDiagnostic(line, i, ref j, tag);

          if (diagnostic != null) {
            break;
          }
        }

        if (diagnostic == null) {
          ++j;
        }
        else {
          diagnostics.Add(diagnostic);
        }
      }
    }

    var parameter = new PublishDiagnosticParams();
    parameter.Uri         = textDocument.Uri;
    parameter.Diagnostics = diagnostics.ToArray();

    if (maxProblems > -1) {
      parameter.Diagnostics = parameter.Diagnostics.Take(maxProblems).ToArray();
    }

    _ = SendMethodNotificationAsync(Methods.TextDocumentPublishDiagnostics, parameter);
  }


  public void SendDiagnostics(Uri uri) {
    if (this.diagnostics == null ||
        !UsePublishModelDiagnostic) {
      return;
    }

    var lines = textDocument.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

    IReadOnlyList<Diagnostic> diagnostics = GetDocumentDiagnostics(lines, null);

    var parameter = new PublishDiagnosticParams();
    parameter.Uri         = uri;
    parameter.Diagnostics = diagnostics.ToArray();

    if (maxProblems > -1) {
      parameter.Diagnostics = parameter.Diagnostics.Take(maxProblems).ToArray();
    }

    _ = SendMethodNotificationAsync(Methods.TextDocumentPublishDiagnostics, parameter);
  }


  public object[] SendReferences(
    ReferenceParams args,
    bool returnLocationsOnly,
    CancellationToken token
  ) {
    Rpc.TraceSource.TraceEvent(
        TraceEventType.Information,
        0,
        $"Received: {JToken.FromObject(args)}"
      );

    IProgress<object[]> progress = args.PartialResultToken;
    var                 delay    = referencesDelay * 1000;

    // Set default values if no custom values are set
    if (referencesChunkSize <= 0 ||
        string.IsNullOrEmpty(referenceToFind)) {
      referenceToFind     = "error";
      referencesChunkSize = 1;
    }

    var referenceWord = referenceToFind;

    if (textDocument == null ||
        progress == null) {
      return Array.Empty<object>();
    }

    var lines = textDocument.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

    var locations = new List<Location>();

    var locationsChunk = new List<Location>();

    for (var i = 0; i < lines.Length; i++) {
      var line = lines[i];

      for (var j = 0; j < line.Length; j++) {
        var location = GetLocation(line, i, ref j, referenceWord);

        if (location != null) {
          locations.Add(location);
          locationsChunk.Add(location);

          if (locationsChunk.Count == referencesChunkSize) {
            Debug.WriteLine($"Reporting references of {referenceWord}");
            Rpc.TraceSource.TraceEvent(
                TraceEventType.Information,
                0,
                $"Report: {JToken.FromObject(locationsChunk)}"
              );
            progress.Report(locationsChunk.ToArray());
            Thread.Sleep(delay); // Wait between chunks
            locationsChunk.Clear();
          }
        }

        if (token.IsCancellationRequested) {
          Debug.WriteLine($"Cancellation Requested for {referenceWord} references");
        }

        token.ThrowIfCancellationRequested();
      }
    }

    // Report last chunk if it has elements since it didn't reached the specified size
    if (locationsChunk.Count() > 0) {
      progress.Report(locationsChunk.ToArray());
      Thread.Sleep(delay); // Wait between chunks
    }

    return locations.ToArray();
  }


  public void SendSettings(DidChangeConfigurationParams parameter) {
    CurrentSettings = parameter.Settings.ToString();
    NotifyPropertyChanged(nameof(CurrentSettings));

    var parsedSettings = JToken.Parse(CurrentSettings);
    var newMaxProblems =
      parsedSettings.Children().First().Values<int>("maxNumberOfProblems").First();
    if (maxProblems != newMaxProblems) {
      maxProblems = newMaxProblems;
      SendDiagnostics();
    }
  }


  public void SetDocumentHighlightsParams(int chunkSize, int delay = 0) {
    highlightChunkSize = chunkSize;
    highlightsDelay    = delay * 1000;
  }


  public void SetDocumentSymbols(IEnumerable<VSSymbolInformation> symbolsInfo) {
    if (textDocument == null) {
      Symbols = Array.Empty<VSSymbolInformation>();
      return;
    }

    var lines = textDocument.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

    for (var i = 0; i < lines.Length; i++) {
      var line = lines[i];

      for (var j = 0; j < line.Length; j++) {
        foreach (var symbolInfo in symbolsInfo) {
          Location loc = null;

          loc = GetLocation(line, i, ref j, symbolInfo.Name);

          if (loc != null) {
            symbolInfo.Location = loc;
          }
        }
      }
    }

    Symbols = symbolsInfo;
  }


  public void SetFindReferencesParams(string wordToFind, int chunkSize, int delay = 0) {
    referenceToFind     = wordToFind;
    referencesChunkSize = chunkSize;
    referencesDelay     = delay;
  }


  public void SetFoldingRanges(IEnumerable<FoldingRange> foldingRanges) {
    FoldingRanges = foldingRanges;
  }


  public void SetProjectContexts(IEnumerable<VSProjectContext> contexts) {
    Contexts = contexts;
  }


  public void ShowMessage(string message, MessageType messageType) {
    var parameter = new ShowMessageParams {
      Message     = message,
      MessageType = messageType
    };
    _ = SendMethodNotificationAsync(Methods.WindowShowMessage, parameter);
  }


  public async Task<MessageActionItem> ShowMessageRequestAsync(
    string message,
    MessageType messageType,
    string[] actionItems
  ) {
    var parameter = new ShowMessageRequestParams {
      Message     = message,
      MessageType = messageType,
      Actions     = actionItems.Select(a => new MessageActionItem { Title = a }).ToArray()
    };

    return await SendMethodRequestAsync(Methods.WindowShowMessageRequest, parameter);
  }


  public void UpdateServerSideTextDocument(string text, int version) {
    if (textDocument != null) {
      textDocument.Text    = text;
      textDocument.Version = version;
    }
  }


  public void WaitForExit() {
    disconnectEvent.WaitOne();
  }


  private VSDiagnostic GetDiagnostic(
    string line,
    int lineOffset,
    ref int characterOffset,
    DiagnosticsInfo diagnosticInfo,
    TextDocumentIdentifier textDocumentIdentifier = null
  ) {
    var              wordToMatch      = diagnosticInfo.Text;
    VSProjectContext context          = diagnosticInfo.Context;
    VSProjectContext requestedContext = null;

    VSDiagnosticProjectInformation projectAndContext = null;
    if (textDocumentIdentifier != null &&
        textDocumentIdentifier is VSTextDocumentIdentifier vsTextDocumentIdentifier &&
        vsTextDocumentIdentifier.ProjectContext != null) {
      requestedContext = vsTextDocumentIdentifier.ProjectContext;
      projectAndContext = new VSDiagnosticProjectInformation {
        ProjectName       = vsTextDocumentIdentifier.ProjectContext.Label,
        ProjectIdentifier = vsTextDocumentIdentifier.ProjectContext.Id,
        Context           = "Win32"
      };
    }

    if (characterOffset + wordToMatch?.Length <= line.Length) {
      var subString = line.Substring(characterOffset, wordToMatch.Length);
      if (subString.Equals(wordToMatch, StringComparison.OrdinalIgnoreCase) &&
          context == requestedContext) {
        var diagnostic = new VSDiagnostic();
        diagnostic.Message = "This is an " +
                             Enum.GetName(typeof(DiagnosticSeverity), diagnosticInfo.Severity);
        diagnostic.Severity    = diagnosticInfo.Severity;
        diagnostic.Range       = new Range();
        diagnostic.Range.Start = new Position(lineOffset, characterOffset);
        diagnostic.Range.End   = new Position(lineOffset, characterOffset + wordToMatch.Length);
        diagnostic.Code =
          "Test" + Enum.GetName(typeof(DiagnosticSeverity), diagnosticInfo.Severity);
        diagnostic.CodeDescription = new CodeDescription {
          Href = new Uri("https://www.microsoft.com")
        };

        if (projectAndContext != null) {
          diagnostic.Projects = new[] { projectAndContext };
        }

        diagnostic.Identifier = lineOffset +
                                "," +
                                characterOffset +
                                " " +
                                lineOffset +
                                "," +
                                diagnostic.Range.End.Character;
        characterOffset = characterOffset + wordToMatch.Length;

        // Our Mock UI only allows setting one tag at a time
        diagnostic.Tags    = new DiagnosticTag[1];
        diagnostic.Tags[0] = diagnosticInfo.Tag;

        return diagnostic;
      }
    }

    return null;
  }


  private IReadOnlyList<VSDiagnostic> GetDocumentDiagnostics(
    string[] lines,
    TextDocumentIdentifier textDocumentIdentifier
  ) {
    var diagnostics = new List<VSDiagnostic>();
    for (var i = 0; i < lines.Length; i++) {
      var line = lines[i];

      var j = 0;
      while (j < line.Length) {
        VSDiagnostic diagnostic = null;
        foreach (var tag in this.diagnostics) {
          diagnostic = GetDiagnostic(line, i, ref j, tag, textDocumentIdentifier);

          if (diagnostic != null) {
            break;
          }
        }

        if (diagnostic == null) {
          ++j;
        }
        else {
          diagnostics.Add(diagnostic);
        }
      }
    }

    return diagnostics;
  }


  private Range GetHighlightRange(
    string line,
    int lineOffset,
    ref int characterOffset,
    string wordToMatch
  ) {
    if (characterOffset + wordToMatch.Length <= line.Length) {
      var subString = line.Substring(characterOffset, wordToMatch.Length);
      if (subString.Equals(wordToMatch, StringComparison.OrdinalIgnoreCase)) {
        var range = new Range();
        range.Start = new Position(lineOffset, characterOffset);
        range.End   = new Position(lineOffset, characterOffset + wordToMatch.Length);

        return range;
      }
    }

    return null;
  }


  private Location GetLocation(
    string line,
    int lineOffset,
    ref int characterOffset,
    string wordToMatch
  ) {
    if (characterOffset + wordToMatch.Length <= line.Length) {
      var subString = line.Substring(characterOffset, wordToMatch.Length);
      if (subString.Equals(wordToMatch, StringComparison.OrdinalIgnoreCase)) {
        var location = new Location();
        location.Uri         = textDocument.Uri;
        location.Range       = new Range();
        location.Range.Start = new Position(lineOffset, characterOffset);
        location.Range.End   = new Position(lineOffset, characterOffset + wordToMatch.Length);

        return location;
      }
    }

    return null;
  }


  private string GetWordAtPosition(Position position, string[] lines) {
    var line = lines.ElementAtOrDefault(position.Line);

    var result = new StringBuilder();

    var startIdx = position.Character;
    var endIdx   = startIdx + 1;

    while (char.IsLetter(line.ElementAtOrDefault(startIdx))) {
      result.Insert(0, line[startIdx]);
      startIdx--;
    }

    while (char.IsLetter(line.ElementAtOrDefault(endIdx))) {
      result.Append(line[endIdx]);
      endIdx++;
    }

    return result.ToString();
  }


  private void NotifyPropertyChanged(string propertyName) {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }


  private void OnRpcDisconnected(object sender, JsonRpcDisconnectedEventArgs e) {
    Exit();
  }


  private void OnTargetInitializeCompletion(object sender, EventArgs e) {
    var timer = new Timer(LogMessage, null, 0, 5 * 1000);
  }


  private void OnTargetInitialized(object sender, EventArgs e) {
    OnInitialized?.Invoke(this, EventArgs.Empty);
  }


  private Task SendMethodNotificationAsync<TIn>(LspNotification<TIn> method, TIn param) {
    return Rpc.NotifyWithParameterObjectAsync(method.Name, param);
  }


  private Task<TOut> SendMethodRequestAsync<TIn, TOut>(LspRequest<TIn, TOut> method, TIn param) {
    return Rpc.InvokeWithParameterObjectAsync<TOut>(method.Name, param);
  }
}

public class DiagnosticsInfo {
  public string Text { get; set; }

  public VSProjectContext Context { get; set; }

  public DiagnosticTag Tag { get; set; }

  public DiagnosticSeverity Severity { get; set; }


  public DiagnosticsInfo(
    string Text,
    VSProjectContext context,
    DiagnosticTag tag,
    DiagnosticSeverity severity
  ) {
    this.Text = Text;
    Context   = context;
    Tag       = tag;
    Severity  = severity;
  }
}

public class SymbolInfo {
  public string Name { get; set; }

  public SymbolKind Kind { get; set; }

  public string Container { get; set; }


  public SymbolInfo(string name, SymbolKind kind, string container) {
    Name      = name;
    Kind      = kind;
    Container = container;
  }
}
