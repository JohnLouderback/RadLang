import React, { FC, useState } from 'react';

import { Text } from 'ink';
import Select from 'ink-select-input';
import Spinner from 'ink-spinner';
import { configure } from 'mobx';

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
  const [selectedCommand, setSelectedCommand] =
    useState<Nullable<Command>>(command);

  // If no command is selected, show the command selection menu.
  if (!selectedCommand) {
    return (
      <Select
        items={[
          {
            label:
              'init: Initializes all support requirements for the monorepo.',
            value: Command.Init
          }
        ]}
        onSelect={(item) => setSelectedCommand(item.value)}
      />
    );
  } else if (selectedCommand === Command.Init) {
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
