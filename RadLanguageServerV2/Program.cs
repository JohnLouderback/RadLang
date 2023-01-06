// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using RadLanguageServerV2;
using RadLanguageServerV2.Handlers;
using RadLanguageServerV2.Services;

Debugger.Launch();
while (!Debugger.IsAttached) {
  await Task.Delay(100);
}

var inputStream  = Console.OpenStandardInput();
var outputStream = Console.OpenStandardOutput();

await Task.Run(
    () => {
      var exitSignal = new TaskCompletionSource<bool>();

      new LanguageServer()
        .WithHandler<
          DidOpenTextDocumentParams,
          DidOpenTextDocumentHandler
        >(Methods.TextDocumentDidOpenName)
        .WithHandler<
          DidChangeTextDocumentParams,
          DidChangeTextDocumentHandler
        >(Methods.TextDocumentDidChangeName)
        .WithHandler<
          TextDocumentPositionParams,
          Hover?,
          DidHoverTextDocumentHandler
        >(Methods.TextDocumentHoverName)
        .WithHandler<
          SemanticTokensParams,
          SemanticTokens,
          SemanticTokensHandler
        >(Methods.TextDocumentSemanticTokensFullName)
        .WithHandler<
          DocumentSymbolParams,
          DocumentSymbol[],
          TextDocumentSymbolHandler
        >(Methods.TextDocumentDocumentSymbolName)
        .WithService<DocumentManagerService>()
        .Start(inputStream, outputStream);

      return exitSignal.Task;
    }
  );

Console.WriteLine("Exiting...");
