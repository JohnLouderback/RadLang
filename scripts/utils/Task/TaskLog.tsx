import React, { FC } from 'react';

import { Box, DOMElement, measureElement, Text } from 'ink';
import Color from 'ink-color-pipe';
import { observer } from 'mobx-react-lite';

interface ITaskLogProps {
  /**
   * The log for the task to display.
   */
  log: string[];
  /**
   * The initial maximum width of the output.
   */
  outputWidth: number;
  /**
   * The last `n` lines of the log to display.
   */
  outputHeight: number;
  /** The parent element of the output. */
  parent: DOMElement;
}

/** Defines the color of the wrap character */
const wrapCharStyle = 'gray';

/** Defines the Unicode character for the carriage return symbol */
const wrapChar = '\u21b5';

export const TaskLog: FC<ITaskLogProps> = observer(({ log, parent }) => {
  if (!parent) return null;
  const { height: outputHeight, width: outputWidth } = measureElement(parent);
  // The maximum length of a line of text. This is the output width minus 2 for the box border.
  const maxLength = outputWidth - 2;
  return (
    <>
      {log
        // First, slice the log to the last `n` lines of the log minus 2 for the box
        // border. We do this now and at the end as well. This is because the wrapped
        // lines may add more lines than the original log entry but we still want to
        // minimize the number of line we do the following operations on.
        .slice(-(outputHeight - 2))
        // Then, split the lines by newline characters since any log entry with a
        // newline character should be split into multiple log entries.
        .flatMap((str) =>
          str.split(/(\r\n|\n\r|\r|\n)/g).flatMap((s, i) => {
            // initialize an empty array to hold the wrapped lines
            const wrappedLines = [];
            // initialize a variable to hold the remaining part of the current line
            let remaining = s.trim();

            if (remaining.length === 0) {
              return [<Text> </Text>];
            }

            // repeat until the remaining line is shorter than maxLength
            while (remaining.length > maxLength) {
              // push a wrapped line onto the array
              wrappedLines.push(
                <>
                  <Text>
                    <Color>{remaining.slice(0, maxLength - 1)}</Color>
                    <Color styles={wrapCharStyle}>{wrapChar}</Color>
                  </Text>
                </>
              );
              // slice off the wrapped part of the line
              remaining = remaining.slice(maxLength - 1);
            }

            // push the remaining part of the line (shorter than maxLength) onto the array
            wrappedLines.push(<Text>{remaining}</Text>);
            // return the array of wrapped lines
            return wrappedLines;
          })
        )
        // Finally, slice the wrapped lines to the last `n` lines of the log minus 2
        // for the box border again since the wrapped lines may have added more lines
        // than the original log entry.
        .slice(-(outputHeight - 2))
        .map((item, i) => (
          <Box key={i} width="100%">
            {/* set the max width of the text to the output width minus 2 for the box border */}
            {item}
          </Box>
        ))}
    </>
  );
});
