import * as path from 'path';
import {
  ExtensionContext,
  window,
  workspace,
} from 'vscode';
import {
  CloseAction,
  ErrorAction,
  ErrorHandlerResult,
  LanguageClient,
  LanguageClientOptions,
  Message,
  ServerOptions,
  TransportKind,
} from 'vscode-languageclient/node';

let client: LanguageClient;

export function activate(context: ExtensionContext) {
  const out = window.createOutputChannel('Rad');

  // The server is implemented in node
  const serverExecutable = context.asAbsolutePath(
    path.join(
      '..',
      '..',
      '..',
      'Projects',
      'RadLang',
      'RadLanguageServerV2',
      'bin',
      'Debug',
      'net7.0',
      'RadLanguageServerV2.exe',
    ),
  );
  // The debug options for the server
  // --inspect=6009: runs the server in Node's Inspector mode so VS Code can attach to the server for debugging
  const debugOptions = { execArgv: ['--nolazy', '--inspect=6009'] };

  // If the extension is launched in debug mode then the debug server options are used
  // Otherwise the run options are used
  const serverOptions: ServerOptions = {
    run: { command: serverExecutable, transport: TransportKind.stdio },
    debug: {
      command: serverExecutable,
      transport: TransportKind.stdio,
      ...debugOptions,
    },
  };

  // Options to control the language client
  const clientOptions: LanguageClientOptions = {
    // Register the server for plain text documents
    documentSelector: [{ language: 'rad' }],
    synchronize: {
      // Notify the server about file changes to '.clientrc files contained in the workspace
      fileEvents: workspace.createFileSystemWatcher('**/.clientrc'),
    },
    errorHandler: {
      error(error: Error, message?: Message): ErrorHandlerResult {
        return {
          action: ErrorAction.Shutdown,
          message: `An error occurred on the Rad Language Server: "${message}"`,
        };
      },
      closed() {
        return {
          action: CloseAction.DoNotRestart,
          message:
            'An error occurred on the Rad Language Server and the connection to the server was closed as a result.',
        };
      },
    },
  };

  out.appendLine(`Starting LSP client for server at "${serverExecutable}".`);

  // Create the language client and start the client.
  client = new LanguageClient(
    'radLangServer',
    'Rad - Language Server',
    serverOptions,
    clientOptions,
  );

  // Start the client. This will also launch the server
  client.start();
}

export async function deactivate(): Promise<void> {
  if (client) client.stop();
}
