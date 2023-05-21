import React, { FC, useState } from 'react';

import { render, Text } from 'ink';
import Select from 'ink-select-input';
import Spinner from 'ink-spinner';
import { configure } from 'mobx';

import { EmscriptenInstallUI } from './init/emscripten/install.js';
import { Nullable } from './utils/type-utils.js';

// Disable MobX's strict mode.
configure({
  enforceActions: 'never'
});

/**
 * Enumerates the commands that can be run by the CLI.
 */
enum Command {
  Init = 'init',
  Install = 'install'
}

interface IAppUIProps {
  /**
   * The command to run.
   */
  command?: Nullable<Command>;
}

const AppUI: FC<IAppUIProps> = ({ command }) => {
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
    return <EmscriptenInstallUI />;
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

render(<AppUI />);
