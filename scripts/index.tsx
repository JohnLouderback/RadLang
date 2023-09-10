import React from 'react';

import { render } from 'ink';
import yargs from 'yargs';
import { hideBin } from 'yargs/helpers';

import {
  AppUI,
  Command,
} from './AppUI.js';
import { Nullable } from './utils/type-utils.js';

const renderApp = (command: Nullable<Command>) => {
  render(<AppUI command={command} />);
};

yargs(hideBin(process.argv))
  .command('init', 'Initialize the project', (yargs) => {
    const { argv } = yargs;
    renderApp(Command.Init);
  })
  .options({
    'non-interactive': {
      alias: 'n',
      type: 'boolean',
      description:
        'Run the command in non-interactive mode. Additionally this use a simplified output format.'
    }
  })
  .help().argv;
