import * as path from 'path';
import { dirname } from 'path';
import { fileURLToPath } from 'url';

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

/** Represents the root directory for the entire project. */
export const monorepoRoot = path.resolve(__dirname, '../../');

/**
 * Represents the "support" directory for the project. The support directory is used to
 * store any files or utilities that are used for the project but are not part of the
 * final output. For instance, we require Emscripten to build the runtime library, but
 * we don't want to include Emscripten in the final output.
 */
export const supportDir = path.resolve(monorepoRoot, '.support/');
