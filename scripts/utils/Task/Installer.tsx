import React, {
  FC,
  Ref,
  useCallback,
  useEffect,
  useLayoutEffect,
  useState
} from 'react';

import { Box, DOMElement, measureElement, useStdout } from 'ink';
import { autorun } from 'mobx';
import { observer } from 'mobx-react-lite';

import useStdoutDimensions from '../../hooks/useStdoutDimensions.js';
import { runAsyncFunctionsInOrder } from '../function-utils.js';
import { CurrentTasks } from './CurrentTasks.js';
import { ExecutableTask } from './ExecutorTask.js';
import { ITaskConstructor } from './ITask.js';
import { ParentTask } from './ParentTask.js';
import { TaskLog } from './TaskLog.js';
import { TaskTree } from './TaskTree.js';

interface IInstallerProps {
  /**
   * The main task representing the installation.
   */
  task: ITaskConstructor;
  /**
   * Specifies if the CLI was run in non-interactive mode. Defaults to `false`. However,
   * if either the output is not TTY (such as when it is redirected) or the `CI` environment
   * variable is set, then this value will be overridden to `true`.
   */
  nonInteractive?: boolean;
}

/**
 * Determines if the current environment is interactive. For example, if the current
 * environment is a TTY and not a CI environment, then it is interactive.
 */
const isInteractive = process.stdout.isTTY && !process.env['CI'];

export const Installer: FC<IInstallerProps> = observer(
  ({ task: taskConstructor }) => {
    const [columns, rows] = useStdoutDimensions();
    const outputRef = React.useRef<typeof Box>(null);
    const [outputHeight, setOutputHeight] = useState(0);
    const [outputWidth, setOutputWidth] = useState(0);
    const [task] = useState(
      'subTasks' in taskConstructor
        ? new ParentTask(taskConstructor)
        : new ExecutableTask(taskConstructor)
    );
    const { stdout } = useStdout();
    const [lastMessage, setLastMessage] = useState('');
    const [lastMessageWasRendered, setLastMessageWasRendered] = useState(true);

    const updateLogSizes = useCallback(() => {
      if (outputRef.current) {
        const { height, width } = measureElement(
          outputRef.current as unknown as DOMElement
        );
        if (height !== outputHeight) setOutputHeight(height);
        if (width !== outputWidth) setOutputWidth(width);
      }
    }, [outputRef.current]);

    useEffect(() => {
      // Run all the tasks in order. If the task is a parent task, then run all the
      // subtasks in order.
      if (task instanceof ParentTask) {
        runAsyncFunctionsInOrder(
          task.executors.map((task) => [task.execute, task])
        );
      }
      // Otherwise, if the task is an executable task, then run the task.
      else if (task instanceof ExecutableTask) {
        task.execute();
      }

      // When the last message to be logged changes, update the last message. In
      // non-interactive terminals (e.g. CI environments), this will be used to render the
      // last logged message to the terminal.
      return autorun(() => {
        // If the terminal is interactive, then don't do anything.
        if (isInteractive) return;

        // Otherwise, get the last message from the log and set it as the last message.
        // This will be rendered to the terminal at next render of the component.
        const lastLog = task.log.at(-1)!;
        if (lastMessage !== lastLog) {
          setLastMessage(lastLog);
          setLastMessageWasRendered(false);
        }
      });
    }, []);

    // If the terminal is not interactive, then log the last message to the terminal if it
    // has not already been rendered.
    if (!isInteractive && lastMessage && !lastMessageWasRendered) {
      stdout.write(lastMessage + '\n');
      setLastMessageWasRendered(true);
    }

    // Update the log sizes when the output dimensions change.
    useLayoutEffect(() => {
      updateLogSizes();
    });

    // If the terminal is not interactive, then don't render anything. We'll just log the
    // last message to the terminal using the `useStdout` hook above.
    return !isInteractive ? null : (
      <>
        <Box width={columns} height={rows} flexDirection="column">
          <Box flexDirection="row" flexGrow={1}>
            <Box
              ref={outputRef as unknown as Ref<DOMElement> | undefined}
              borderStyle="round"
              borderColor="green"
              flexDirection="column"
              flexWrap="wrap"
              flexGrow={2}
              overflowX="hidden"
              // Subtract 2 from the height to account for the border. Then if the top-level
              // task is not a parent task, subtract 1 more for the task name. Otherwise, if
              // it is a parent task, subtract the number of all tasks from the height. This
              // guarantees enough room for the current task list.
              height={
                rows -
                2 -
                (task instanceof ParentTask ? task.allTasks.length : 1)
              }
              width="75%"
            >
              <TaskLog
                log={task.log}
                outputWidth={outputWidth}
                outputHeight={outputHeight}
                parent={outputRef.current as unknown as DOMElement}
              />
            </Box>
            <Box borderStyle="round" borderColor="green" flexGrow={1}>
              <TaskTree task={task} />
            </Box>
          </Box>
          <Box
            borderStyle="round"
            borderColor="green"
            flexGrow={1}
            flexShrink={1}
            alignItems="flex-start"
            justifyContent="flex-start"
          >
            <CurrentTasks task={task} />
          </Box>
        </Box>
      </>
    );
  }
);
