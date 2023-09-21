import { existsSync } from 'fs';
import path from 'path';

import * as Tween from '@tweenjs/tween.js';

import {
  emPythonVersion,
  emSDKDir,
  emVersion
} from '../../../utils/constants.js';
import { isDirEmpty, supportDir } from '../../../utils/directory-utils.js';
import {
  downloadFile,
  extractZipFile,
  givePermissionsToFile,
  makeDirectory
} from '../../../utils/file-utils.js';
import { isWindows } from '../../../utils/platform-utils.js';
import { runCommandAsync } from '../../../utils/process-utils.js';
import { ITaskConstructor } from '../../../utils/Task/ITask.js';
import { ProgressUpdater } from '../../../utils/Task/ProgressUpdater.js';
import { activateEmscriptenTask } from './activate.js';

/** The path to Emscripten SDK in the support directory. */
const emSDKInstallScript = path.resolve(
  emSDKDir,
  './emsdk' + (isWindows() ? '.bat' : '')
);

/** The path to the downloaded Emscripten SDK zip file. */
const emSDKZip = path.resolve(supportDir, 'emsdk.zip');

/** A task which defines the steps needed to install Emscripten. */
export const installEmscriptenTask = {
  name: 'Installing Emscripten',
  shouldSkip: (log) => {
    // If the emsdk directory exists and is not empty, we can skip this task. We'll assume
    // that if the directory exists, it's already been installed.
    if (existsSync(emSDKDir) && !isDirEmpty(emSDKDir)) {
      log('Emscripten SDK is already installed. Skipping installation.');
      return true;
    }
    return false;
  },
  subTasks: [
    {
      name: 'Downloading Emscripten',
      shouldSkip: (log) => {
        // If the emsdk zip file exists, we can skip this task. We'll assume that if the zip
        // file exists, it's already been downloaded.
        if (existsSync(emSDKZip)) {
          log(
            'Emscripten SDK is already downloaded. Skipping download of `emsdk.zip`.'
          );
          return true;
        }
        return false;
      },
      async executor(setProgress, log) {
        await downloadFile(
          'https://github.com/emscripten-core/emsdk/archive/refs/heads/main.zip',
          path.resolve(supportDir, 'emsdk.zip'),
          (downloadedBytes, totalBytes) => {
            setProgress((downloadedBytes / totalBytes) * 100);
          }
        );
      }
    },
    {
      name: 'Extracting Emscripten',
      shouldSkip: (log) => {
        // If the emsdk directory exists and is not empty, we can skip this task. We'll assume
        // that if the directory exists, it's already been installed.
        if (existsSync(emSDKDir) && !isDirEmpty(emSDKDir)) {
          log(
            'Emscripten SDK is already extracted. Skipping extraction of zip file.'
          );
          return true;
        }
        return false;
      },
      async executor(setProgress, log) {
        log('Giving user permissions to extract the zip file.');
        await givePermissionsToFile(emSDKZip);

        log('Ensuring the emsdk directory exists. Creating if not.');
        makeDirectory(emSDKDir);

        log('Attempting to extract the zip file.');
        let lastFileName = '';
        await extractZipFile(emSDKZip, emSDKDir, (fileName, progress) => {
          if (fileName !== lastFileName) {
            log(`Extracting ${fileName}`);
            lastFileName = fileName;
          }
          setProgress(progress);
        });
      }
    },
    {
      name: 'Running Emscripten Install Script',
      async executor(setProgress, log) {
        log(`Giving user permissions to run the ${emSDKInstallScript}.`);
        await givePermissionsToFile(emSDKInstallScript);
        setProgress(10);

        const progressTween = new ProgressUpdater(setProgress, 10).animateTo(
          80,
          120000,
          Tween.Easing.Circular.Out
        );

        log('Installing emscripten.');
        await runCommandAsync(
          emSDKInstallScript,
          ['install', emVersion],
          (stdout) => {
            stdout.split('\n').forEach((line) => {
              //
              if (!line.trim().match(/^[[\]-]+/gm)) {
                log(line.trim());
              }
            });
          },
          (stderr) => {
            stderr.split('\n').forEach((line) => {
              log(line);
            });
          }
        );

        // Wait for the progress to reach 100%.
        await progressTween
          .animateTo(100, 1000, Tween.Easing.Circular.In)
          .waitForAnimation();
      }
    },
    {
      name: 'Installing Python for Emscripten',
      async executor(setProgress, log) {
        const progressTween = new ProgressUpdater(setProgress, 10).animateTo(
          80,
          120000,
          Tween.Easing.Circular.Out
        );

        await runCommandAsync(
          emSDKInstallScript,
          ['install', emPythonVersion],
          (stdout) => {
            stdout.split('\n').forEach((line) => {
              //
              if (!line.trim().match(/^[[\]-]+/gm)) {
                log(line.trim());
              }
            });
          },
          (stderr) => {
            stderr.split('\n').forEach((line) => {
              log(line);
            });
          }
        );

        // Wait for the progress to reach 100%.
        await progressTween
          .animateTo(100, 1000, Tween.Easing.Circular.In)
          .waitForAnimation();
      }
    },
    activateEmscriptenTask
  ]
} satisfies ITaskConstructor;
