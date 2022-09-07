using MediatR;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Progress;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.WorkDone;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;
using RadLanguageServer.Constructs;
using RadLanguageServer.Services;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace RadLanguageServer.Handlers;

internal class TextDocumentHandler : TextDocumentSyncHandlerBase {
  private readonly ILogger<TextDocumentHandler> logger;
  private readonly ILanguageServerConfiguration configuration;
  private readonly DocumentManagerService documentManagerService;
  private readonly ILanguageServerFacade facade;
  private readonly DiagnosticsService diagnosticsService;

  private readonly DocumentSelector _documentSelector = new(
      DocumentFilter.ForLanguage("rad")
    );

  public TextDocumentSyncKind Change { get; } = TextDocumentSyncKind.Full;


  public TextDocumentHandler(
    ILogger<TextDocumentHandler> logger,
    DocumentManagerService documentManagerService,
    DiagnosticsService diagnosticsService,
    ILanguageServerConfiguration configuration,
    ILanguageServerFacade facade
  ) {
    this.logger                 = logger;
    this.configuration          = configuration;
    this.documentManagerService = documentManagerService;
    this.facade                 = facade;
    this.diagnosticsService     = diagnosticsService;
  }


  public override TextDocumentAttributes GetTextDocumentAttributes(DocumentUri uri) {
    return new TextDocumentAttributes(uri, "rad");
  }


  public override Task<Unit> Handle(
    DidChangeTextDocumentParams notification,
    CancellationToken token
  ) {
    var contentChanges = notification.ContentChanges.ToArray();
    // If the content change is the full document.
    if (contentChanges.Length == 1 &&
        contentChanges[0].Range == null) {
      var change = contentChanges[0].Text;

      // Check if the the document already exists.
      if (documentManagerService.Documents.TryGetValue(
              notification.TextDocument.Uri,
              out var document
            )) {
        // If it does, update it.
        document.Update(change);
      }
      // Otherwise, create a new document.
      else {
        documentManagerService.Documents.Add(
            notification.TextDocument.Uri,
            new DocumentContent
              (change)
          );
      }
    }

    Task.Yield();

    // Diagnostic service must be called _after_ the document is added or updated in the document
    // manager.
    diagnosticsService.PublishDiagnostics(notification.TextDocument);

    return Unit.Task;
  }


  public override async Task<Unit> Handle(
    DidOpenTextDocumentParams notification,
    CancellationToken token
  ) {
    documentManagerService.Documents.Add(
        notification.TextDocument.Uri,
        new DocumentContent
          (notification.TextDocument.Text)
      );

    Task.Yield();

    // Diagnostic service must be called _after_ the document is added to the document manager.
    diagnosticsService.PublishDiagnostics(notification.TextDocument);
    return Unit.Value;
  }


  public override Task<Unit> Handle(
    DidCloseTextDocumentParams notification,
    CancellationToken token
  ) {
    documentManagerService.Documents.Remove(notification.TextDocument.Uri);

    return Unit.Task;
  }


  public override Task<Unit> Handle(
    DidSaveTextDocumentParams notification,
    CancellationToken token
  ) {
    return Unit.Task;
  }


  protected override TextDocumentSyncRegistrationOptions CreateRegistrationOptions(
    SynchronizationCapability capability,
    ClientCapabilities clientCapabilities
  ) {
    return new TextDocumentSyncRegistrationOptions {
      DocumentSelector = _documentSelector,
      Change           = Change,
      Save             = new SaveOptions { IncludeText = true }
    };
  }
}

internal class MyWorkspaceSymbolsHandler : IWorkspaceSymbolsHandler {
  private readonly IServerWorkDoneManager _serverWorkDoneManager;
  private readonly IProgressManager _progressManager;
  private readonly ILogger<MyWorkspaceSymbolsHandler> _logger;


  public MyWorkspaceSymbolsHandler(
    IServerWorkDoneManager serverWorkDoneManager,
    IProgressManager progressManager,
    ILogger<MyWorkspaceSymbolsHandler> logger
  ) {
    _serverWorkDoneManager = serverWorkDoneManager;
    _progressManager       = progressManager;
    _logger                = logger;
  }


  public WorkspaceSymbolRegistrationOptions GetRegistrationOptions(
    WorkspaceSymbolCapability capability,
    ClientCapabilities clientCapabilities
  ) {
    return new WorkspaceSymbolRegistrationOptions();
  }


  public async Task<Container<SymbolInformation>> Handle(
    WorkspaceSymbolParams request,
    CancellationToken cancellationToken
  ) {
    using var reporter = _serverWorkDoneManager.For(
        request,
        new WorkDoneProgressBegin {
          Cancellable = true,
          Message     = "This might take a while...",
          Title       = "Some long task....",
          Percentage  = 0
        }
      );
    using var partialResults = _progressManager.For(request, cancellationToken);
    if (partialResults != null) {
      await Task.Delay(2000, cancellationToken).ConfigureAwait(false);

      reporter.OnNext(
          new WorkDoneProgressReport {
            Cancellable = true,
            Percentage  = 20
          }
        );
      await Task.Delay(500, cancellationToken).ConfigureAwait(false);

      reporter.OnNext(
          new WorkDoneProgressReport {
            Cancellable = true,
            Percentage  = 40
          }
        );
      await Task.Delay(500, cancellationToken).ConfigureAwait(false);

      reporter.OnNext(
          new WorkDoneProgressReport {
            Cancellable = true,
            Percentage  = 50
          }
        );
      await Task.Delay(500, cancellationToken).ConfigureAwait(false);

      partialResults.OnNext(
          new[] {
            new SymbolInformation {
              ContainerName = "Partial Container",
              Deprecated    = true,
              Kind          = SymbolKind.Constant,
              Location = new Location {
                Range = new Range(
                    new Position(2, 1),
                    new Position(2, 10)
                  )
              },
              Name = "Partial name"
            }
          }
        );

      reporter.OnNext(
          new WorkDoneProgressReport {
            Cancellable = true,
            Percentage  = 70
          }
        );
      await Task.Delay(500, cancellationToken).ConfigureAwait(false);

      reporter.OnNext(
          new WorkDoneProgressReport {
            Cancellable = true,
            Percentage  = 90
          }
        );

      partialResults.OnCompleted();
      return new SymbolInformation[] {};
    }

    try {
      return new[] {
        new SymbolInformation {
          ContainerName = "Container",
          Deprecated    = true,
          Kind          = SymbolKind.Constant,
          Location = new Location {
            Range = new Range(
                new Position(1, 1),
                new Position(1, 10)
              )
          },
          Name = "name"
        }
      };
    }
    finally {
      reporter.OnNext(
          new WorkDoneProgressReport {
            Cancellable = true,
            Percentage  = 100
          }
        );
    }
  }
}
