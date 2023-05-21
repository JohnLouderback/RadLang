import { exec, execFile } from 'child_process';

import { isWindows } from './platform-utils.js';

/**
 * Runs a command asynchronously.
 * @param command - The command to run.
 * @param args - The arguments to pass to the command. Use an empty array if no arguments are needed.
 * @param stdout - A callback that will be called when stdout data is received.
 * @param stderr - A callback that will be called when stderr data is received.
 * @returns A promise that resolves when the command has finished running. The promise
 * will resolve with the exit code of the command.
 * @example
 * ```ts
 * // Run a command and print the output to the console.
 * await runCommandAsync('ls', ['-la'], console.log, console.error);
 * ```
 */
export const runCommandAsync = async (
  command: string,
  args: Array<string>,
  stdout?: (data: string) => void,
  stderr?: (data: string) => void
): Promise<number> => {
  return new Promise((resolve, reject) => {
    // Use exec on Windows because it can handle .cmd and .bat files. On Unix-like systems, use execFile.
    const childProcess = isWindows()
      ? exec(command + ' ' + args.join(' '))
      : execFile(command, args);
    childProcess.on('error', (error) => {
      reject(error);
    });
    childProcess.on('exit', (code) => {
      resolve(code ?? 0);
    });

    // If stdout callback is provided, pipe the output to it.
    if (stdout) {
      childProcess.stdout?.on('data', (data) => {
        stdout(data.toString());
      });
    }

    // If stderr callback is provided, pipe the output to it.
    if (stderr) {
      childProcess.stderr?.on('data', (data) => {
        stderr(data.toString());
      });
    }
  });
};
