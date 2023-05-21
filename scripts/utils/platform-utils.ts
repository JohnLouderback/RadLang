/**
 * Returns `true` if the current platform is Windows.
 */
export const isWindows = () => {
  return process.platform === 'win32';
};
