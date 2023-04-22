// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using RadLanguageServerV2;
using RadLanguageServerV2.Handlers;
using RadLanguageServerV2.LanguageServerEx;
using RadLanguageServerV2.LanguageServerEx.Models.InlayHint;
using RadLanguageServerV2.LanguageServerEx.Params.InlayHint;
using RadLanguageServerV2.Services;

// This allows us to connect to the language server from VSCode where VSCode will launch the
// language server and the following lines will pause execution until the debugger is attached.
// This is useful so that we can debug the language server remotely without having any code run
// before we can attach the debugger.
#if DEBUG
Debugger.Launch();
while (!Debugger.IsAttached) {
  await Task.Delay(100);
}
#endif

// Obtain references to the standard input and output streams so that we can pass them to the
// language server for message passing.
var inputStream  = Console.OpenStandardInput();
var outputStream = Console.OpenStandardOutput();

// Start the language server and wait for it to signal to exit before exiting the program.
await Task.Run(
    () => {
      // Create a task completion source that will be used to signal when the language server
      // should exit.
      var exitSignal = new TaskCompletionSource<bool>();

      // Create a language server and register handlers for the messages that we want to handle.
      // Additionally, register services that we want to be able to access from our handlers.
      // Finally, start the language server and pass it the input and output streams to use for
      // message passing as our language server will be communicating with VSCode over these
      // streams.
      new LanguageServer()
        // Handler for handling the `textDocument/didOpen` request. Used for when a document is
        // opened in the editor.
        .WithHandler<
          DidOpenTextDocumentParams,
          DidOpenTextDocumentHandler
        >(Methods.TextDocumentDidOpenName)
        // Handler for handling the `textDocument/didChange` request. Used for when a document is
        // modified in the editor.
        .WithHandler<
          DidChangeTextDocumentParams,
          DidChangeTextDocumentHandler
        >(Methods.TextDocumentDidChangeName)
        // Handler for handling the `textDocument/hover` request. Used for when the user hovers
        // over a symbol in the editor.
        .WithHandler<
          TextDocumentPositionParams,
          Hover?,
          DidHoverTextDocumentHandler
        >(Methods.TextDocumentHoverName)
        // Handler for handling the `textDocument/semanticTokens/full` request. Used for when the
        // user requests semantic tokens for a document. This is used for syntax highlighting
        // beyond the basic syntax highlighting that is provided by the language extension's grammar.
        // In Rad, this is important as many of the keywords are contextual and the language server
        // is responsible for determining the meaning of a token based on its context. What might be
        // a keyword in one context might be a variable in another context.
        .WithHandler<
          SemanticTokensParams,
          SemanticTokens,
          SemanticTokensHandler
        >(Methods.TextDocumentSemanticTokensFullName)
        // Handler for handling the `textDocument/documentSymbol` request. Used for when the user
        // requests a list of symbols in the document. This is used for the outline view in the
        // editor.
        .WithHandler<
          DocumentSymbolParams,
          DocumentSymbol[],
          TextDocumentSymbolHandler
        >(Methods.TextDocumentDocumentSymbolName)
        // Handler for handling the `textDocument/inlayHint` request. Used for when the user requests
        // a list of inlay hints for a document. This is used for displaying type information in the
        // editor, such as showing parameter types when calling a function.
        .WithHandler<
          InlayHintParams,
          InlayHint[],
          GetInlayHintsHandler
        >(MethodsEx.InlayHintsName)
        // Registers the document manager service. This service is responsible for managing the
        // documents that are open in the editor.
        .WithService<DocumentManagerService>()
        // Registers the diagnostics service. This service is responsible for managing the diagnostics
        // that are reported for a document, such as syntax errors and type errors.
        .WithService<DiagnosticsService>()
        // Finally start the language server and pass it the input and output streams to use for
        // message passing. The language server uses stdin and stdout for message passing to and from
        // the editor.
        .Start(inputStream, outputStream);

      // Return the task completion source's task so that we can wait for it to signal to exit.
      return exitSignal.Task;
    }
  );

Console.WriteLine("Exiting...");
