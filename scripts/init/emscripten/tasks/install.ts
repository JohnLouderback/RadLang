import path from 'path';

import * as Tween from '@tweenjs/tween.js';

import { emSDKDir, emVersion } from '../../../utils/constants.js';
import { supportDir } from '../../../utils/directory-utils.js';
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

/** A task which defines the steps needed to install Emscripten. */
export const installEmscriptenTask = {
  name: 'Installing Emscripten',
  subTasks: [
    {
      name: 'Downloading Emscripten',
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
      async executor(setProgress, log) {
        const emSDKZip = path.resolve(supportDir, 'emsdk.zip');
        const emSDKDir = path.resolve(supportDir, 'emsdk');

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
        const emSDKInstallScript = path.resolve(
          emSDKDir,
          './emsdk' + (isWindows() ? '.bat' : '')
        );

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
    activateEmscriptenTask
  ]
} satisfies ITaskConstructor;
