import { useEffect, useState } from 'react';

import { useStdout } from 'ink';
import terminalSize from 'term-size';

/**
 * Code adapted from: https://github.com/cameronhunter/ink-monorepo
 */
function useStdoutDimensions(): [number, number] {
  const { stdout } = useStdout();
  const [dimensions, setDimensions] = useState<[number, number]>([
    stdout.columns,
    stdout.rows
  ]);

  useEffect(() => {
    const handler = (columns: number, rows: number) =>
      setDimensions([columns, rows]);
    // Update dimensions on resize
    stdout.on('resize', () => {
      const { columns, rows } = terminalSize();
      handler(columns, rows);
    });
    return () => {
      stdout.off('resize', handler);
    };
  }, [stdout]);

  return dimensions;
}

export default useStdoutDimensions;
