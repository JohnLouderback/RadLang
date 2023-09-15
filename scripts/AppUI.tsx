import React, { FC } from 'react';

import { Text } from 'ink';
import Select from 'ink-select-input';
import Spinner from 'ink-spinner';
import { configure } from 'mobx';

import { useCliArgument } from './components/CLIArgumentContext.js';
import { initRepoTask } from './init/tasks/init.js';
import { Installer } from './utils/Task/Installer.js';
import { Nullable } from './utils/type-utils.js';

// Disable MobX's strict mode.
configure({
  enforceActions: 'never'
});

/**
 * Enumerates the commands that can be run by the CLI.
 */
export enum Command {
  Init = 'init',
  Install = 'install'
}

interface IAppUIProps {
  /**
   * The command to run.
   */
  command?: Nullable<Command>;
  /**
   * Specifies if the CLI was run in non-interactive mode. Defaults to `false`. However,
   * if either the output is not TTY (such as when it is redirected) or the `CI` environment
   * variable is set, then this value will be overridden to `true`.
   */
  nonInteractive?: boolean;
}

export const AppUI: FC<IAppUIProps> = ({ command }) => {
  // The currently selected command.
  const {
    command: currentCommand,
    setCommand,
    cliArguments
  } = useCliArgument();

  // If no command is selected, and we're not in non-interactive mode, show the command selection menu.
  if (!currentCommand) {
    if (!cliArguments['non-interactive']) {
      return (
        <Select
          items={[
            {
              label:
                'init: Initializes all support requirements for the monorepo.',
              value: Command.Init
            }
          ]}
          onSelect={(item) => setCommand(item.value)}
        />
      );
    }
    // If we're in non-interactive mode, then we must have a command selected.
    throw new Error('No command selected.');
  }
  // If the command is `init`, then run the init task.
  else if (currentCommand === Command.Init) {
    return <Installer task={initRepoTask} />;
  }

  return (
    <Text>
      <Text color="green">
        <Spinner type="dots" />
      </Text>
      {' Loading'}
    </Text>
  );
};
