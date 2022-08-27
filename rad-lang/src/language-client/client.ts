import * as path from 'path';
import {
  ExtensionContext,
  window,
  workspace,
} from 'vscode';
import {
  LanguageClient,
  LanguageClientOptions,
  ServerOptions,
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
      'RadLanguageServer',
      'bin',
      'Debug',
      'net7.0',
      'RadLanguageServer.exe',
    ),
  );
  // The debug options for the server
  // --inspect=6009: runs the server in Node's Inspector mode so VS Code can attach to the server for debugging
  const debugOptions = { execArgv: ['--nolazy', '--inspect=6009'] };

  // If the extension is launched in debug mode then the debug server options are used
  // Otherwise the run options are used
  const serverOptions: ServerOptions = {
    run: { command: serverExecutable },
    debug: {
      command: serverExecutable,
    },
  };

  // Options to control the language client
  const clientOptions: LanguageClientOptions = {
    // Register the server for plain text documents
    documentSelector: [{ scheme: 'file', language: 'plaintext' }],
    synchronize: {
      // Notify the server about file changes to '.clientrc files contained in the workspace
      fileEvents: workspace.createFileSystemWatcher('**/.clientrc'),
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
