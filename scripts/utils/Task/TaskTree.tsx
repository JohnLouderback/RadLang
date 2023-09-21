import React, { FC, useMemo } from 'react';

import spinners from 'cli-spinners';
import { Task } from 'ink-task-list';
import { observer } from 'mobx-react-lite';

import { ITask, TaskStatus } from './ITask.js';

interface ITaskListProps {
  /**
   * The main task representing the installation.
   */
  task: ITask;
}

const getTaskStateStringFromTask = (
  status: TaskStatus
): 'loading' | 'pending' | 'success' | 'warning' | 'error' => {
  switch (status) {
    case TaskStatus.Pending:
      return 'pending';
    case TaskStatus.InProgress:
      return 'loading';
    case TaskStatus.Completed:
      return 'success';
    case TaskStatus.Failed:
      return 'error';
    case TaskStatus.Skipped:
      return 'warning';
    default:
      throw new Error(`Unknown task status: ${status}`);
  }
};

const spinner = {
  ...spinners.clock,
  interval: 200
};

export const TaskTree: FC<ITaskListProps> = observer(({ task }) => {
  const taskState = useMemo(
    () => getTaskStateStringFromTask(task.status),
    [task.status]
  );

  return (
    <Task
      label={task.name}
      isExpanded={
        'subTasks' in task &&
        !!task.subTasks.length &&
        (task.status === TaskStatus.InProgress ||
          task.status === TaskStatus.Skipped ||
          task.status === TaskStatus.Completed)
      }
      state={taskState}
      spinner={spinner}
    >
      {'subTasks' in task
        ? task.subTasks.map((subTask, i) => (
            <TaskTree task={subTask} key={subTask.name} />
          ))
        : undefined}
    </Task>
  );
});
