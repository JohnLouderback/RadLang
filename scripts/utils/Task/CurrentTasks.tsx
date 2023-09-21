import React, { FC } from 'react';

import { Box, Text } from 'ink';
import { observer } from 'mobx-react-lite';

import { ProgressBar } from '@inkjs/ui';

import { ITask, TaskStatus } from './ITask.js';

interface ICurrentTasksProps {
  /**
   * The main task representing the installation.
   */
  task: ITask;
}

const getInProgressTasks = (task: ITask): ITask[] => {
  const tasks: Array<ITask | Array<ITask>> = [];
  if (
    task.status === TaskStatus.InProgress ||
    task.status === TaskStatus.Completed
  ) {
    tasks.push(task);
  }
  if ('subTasks' in task) {
    task.subTasks.forEach((subTask) => {
      tasks.push(getInProgressTasks(subTask));
    });
  }

  return (
    tasks
      .flat()
      // Sort the tasks where "completed" tasks come first.
      .sort((a, b) => {
        if (a.status === TaskStatus.Completed) {
          return 1;
        } else if (b.status === TaskStatus.Completed) {
          return -1;
        }
        return 0;
      })
  );
};

export const CurrentTasks: FC<ICurrentTasksProps> = observer(({ task }) => {
  return (
    <Box flexDirection="column">
      {getInProgressTasks(task).map((task) => (
        <Box flexDirection="row" flexGrow={1}>
          <Box width={30}>
            <ProgressBar
              value={task.status !== TaskStatus.Completed ? task.progress : 100}
            />
          </Box>
          <Text> {task.name}</Text>
          <Text>
            {task.status === TaskStatus.Completed
              ? ': Complete!'
              : task.status === TaskStatus.Skipped
              ? ': Skipped!'
              : ' - ' + Math.ceil(task.progress) + '%'}
          </Text>
        </Box>
      ))}
    </Box>
  );
});
