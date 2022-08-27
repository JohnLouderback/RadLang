import * as vscode from 'vscode';

import helloWorld from './helloWorld';
import * as lspClient from './language-client/client';

export function activate(context: vscode.ExtensionContext): void {
  helloWorld();
  lspClient.activate(context);
}

export function deactivate(): void {
  lspClient.deactivate();
}
