import React, { Context, createContext, FC, useContext, useState } from 'react';

import { Nullable } from '../utils/type-utils.js';

/**
 * The possible overloads for the `setArgument` function.
 */
interface ISetArgument {
  /**
   * Sets the CLI arguments from a key-value pair object. Useful for setting multiple arguments at once.
   * @param args - The CLI arguments as key-value pairs.
   * @example
   * setArgument({ key: 'value' });
   */
  (args: Record<string, unknown>): void;
  /**
   * Sets a single CLI argument.
   * @param key - The key of the CLI argument.
   * @param value - The value of the CLI argument.
   * @example
   * setArgument('key', 'value');
   */
  (key: string, value: string): void;
}

/**
 * This context is used to pass CLI arguments to the components.
 */
const CLIArgumentContext: Context<{
  cliArguments: Record<string, unknown>;
  setArgument: ISetArgument;
  command: string;
  setCommand: (command: string) => void;
}> = createContext({
  cliArguments: {},
  setArgument: ((
    key: string | Record<string, unknown>,
    value?: string
  ) => {}) as ISetArgument,
  command: '',
  setCommand: (command: string) => {}
});

/**
 * This hook is used to access the CLI arguments.
 */
export const useCliArgument = () => {
  return useContext(CLIArgumentContext);
};

/**
 * This component is used to provide the CLI arguments to all child components.
 */
export const CLIArgumentProvider: FC<{
  command: Nullable<string>;
  args: Record<string, unknown>;
  children: React.ReactNode;
}> = ({ children, args, command }) => {
  // The CLI arguments stored as state.
  const [cliArguments, setCliArguments] = useState(args);

  // The currently selected command.
  const [cmd, setCommand] = useState(command);

  const setArgument: ISetArgument = (key, value?) => {
    // If the key is a string, then we are setting a single argument.
    if (typeof key === 'string' && typeof value === 'string') {
      // Take the previous arguments and add the new one.
      setCliArguments((prev) => ({ ...prev, [key]: value }));
      return;
    }

    // If the key neither a string nor an object, then throw an error.
    if (typeof key !== 'object') {
      throw new Error(
        'The first argument must be a string or an object. Received ' +
          typeof key
      );
    }

    // Otherwise, we are setting multiple arguments. Merge the previous arguments with the new ones.
    setCliArguments((prev) => ({ ...prev, ...key }));
  };

  return (
    <CLIArgumentContext.Provider
      value={{ cliArguments, setArgument, command: cmd, setCommand }}
    >
      {children}
    </CLIArgumentContext.Provider>
  );
};
