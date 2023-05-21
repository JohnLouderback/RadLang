import path from 'path';

import { supportDir } from './directory-utils.js';

/** The version of Emscripten to install. */
export const emVersion = '3.1.37';

/** The directory where Emscripten will be installed. */
export const emSDKDir = path.resolve(supportDir, 'emsdk/emsdk-main/');
