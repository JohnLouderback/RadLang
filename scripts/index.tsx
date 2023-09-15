import React from 'react';

import { render } from 'ink';
import yargs from 'yargs';
import { hideBin } from 'yargs/helpers';

import { AppUI, Command } from './AppUI.js';
import { CLIArgumentProvider } from './components/CLIArgumentContext.js';
import { Nullable } from './utils/type-utils.js';

const renderApp = (
  command: Nullable<Command>,
  args: Record<string, unknown>
) => {
  render(
    <CLIArgumentProvider args={args} command={command}>
      <AppUI command={command} />
    </CLIArgumentProvider>
  );
};

yargs(hideBin(process.argv))
  .command('init', 'Initialize the project', (yargs) => {
    const { argv } = yargs;
    renderApp(Command.Init, argv as Record<string, unknown>);
  })
  .options({
    'non-interactive': {
      alias: 'n',
      type: 'boolean',
      description:
        'Run the command in non-interactive mode. Additionally this produces a simplified output format.'
    }
  })
  .help().argv;
