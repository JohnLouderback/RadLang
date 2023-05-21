import path from 'path';

import * as Tween from '@tweenjs/tween.js';

import { emSDKDir, emVersion } from '../../../utils/constants.js';
import { givePermissionsToFile } from '../../../utils/file-utils.js';
import { isWindows } from '../../../utils/platform-utils.js';
import { runCommandAsync } from '../../../utils/process-utils.js';
import { ITaskConstructor } from '../../../utils/Task/ITask.js';
import { ProgressUpdater } from '../../../utils/Task/ProgressUpdater.js';

/** A task which defines the steps needed to activate Emscripten. */
export const activateEmscriptenTask = {
  name: 'Running Emscripten Activate Script',
  async executor(setProgress, log) {
    const emSDKActivateScript = path.resolve(
      emSDKDir,
      './emsdk' + (isWindows() ? '.bat' : '')
    );

    log(`Giving user permissions to run the ${emSDKActivateScript}.`);
    await givePermissionsToFile(emSDKActivateScript);
    setProgress(10);

    const progressTween = new ProgressUpdater(setProgress, 10).animateTo(
      80,
      120000,
      Tween.Easing.Circular.Out
    );

    log('Activating emscripten.');
    await runCommandAsync(
      emSDKActivateScript,
      ['activate', emVersion],
      (stdout) => {
        stdout.split('\n').forEach((line) => {
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
} satisfies ITaskConstructor;
