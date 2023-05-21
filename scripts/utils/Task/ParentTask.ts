import { computed, makeObservable, observable } from 'mobx';

import { Nullable } from '../type-utils.js';
import { ExecutableTask } from './ExecutorTask.js';
import {
  IParentTask,
  IParentTaskConstructor,
  ITask,
  TaskStatus
} from './ITask.js';

/**
 * Represents a task that can contain other tasks.
 */
export class ParentTask implements IParentTask {
  /** @inheritdoc */
  public readonly name: string;
  @observable private _log: Array<string> = [];

  /** @inheritdoc */
  @computed public get log() {
    if (this.parent) {
      return this.parent.log;
    } else {
      return this._log;
    }
  }

  public set log(log: Array<string>) {
    if (this.parent) {
      this.parent.log = this.parent.log.concat(log);
    } else {
      this._log = log;
    }
  }

  /** @inheritdoc */
  @computed public get progress() {
    return Math.round(
      this.subTasks.reduce((sum, subTask) => sum + subTask.progress, 0) /
        this.subTasks.length
    );
  }

  /** @inheritdoc */
  @computed public get status() {
    // If any of the subtasks are in progress, this task is in progress.
    if (
      this.subTasks.some((subTask) => subTask.status === TaskStatus.InProgress)
    ) {
      return TaskStatus.InProgress;
    }
    // If any of the subtasks failed, this task failed.
    else if (
      this.subTasks.some((subTask) => subTask.status === TaskStatus.Failed)
    ) {
      return TaskStatus.Failed;
    }
    // If all of the subtasks are pending, this task is pending.
    else if (
      this.subTasks.every((subTask) => subTask.status === TaskStatus.Pending)
    ) {
      return TaskStatus.Pending;
    }
    // If all of the subtasks are completed, this task is completed.
    else if (
      this.subTasks.every((subTask) => subTask.status === TaskStatus.Completed)
    ) {
      return TaskStatus.Completed;
    }
    // If none of the above, this task is in progress.
    else {
      return TaskStatus.InProgress;
    }
  }

  /** @inheritdoc */
  @observable public readonly parent: Nullable<IParentTask>;

  /** @inheritdoc */
  @observable public readonly subTasks: ITask[];

  /**
   * @returns An array of all of the executable tasks in this task and its subtasks recursively.
   */
  @computed public get executors(): ExecutableTask[] {
    const tasks: Array<ExecutableTask | Array<ExecutableTask>> = [];

    // For each subtask, if it is an executable task, add it to the list of tasks.
    this.subTasks.forEach((subTask) => {
      if (subTask instanceof ExecutableTask) {
        tasks.push(subTask);
      }
      // Otherwise, if it is a parent task, add its executors to the list of tasks.
      else if (subTask instanceof ParentTask) {
        tasks.push(subTask.executors);
      }
    });

    return tasks.flat();
  }

  /**
   * @returns An array of all of the tasks in this task and its subtasks recursively,
   * including this task and other parent tasks.
   */
  @computed public get allTasks(): ITask[] {
    const tasks: Array<ITask | Array<ITask>> = [this];

    // For each subtask, if it is an executable task, add it to the list of tasks.
    this.subTasks.forEach((subTask) => {
      if (subTask instanceof ExecutableTask) {
        tasks.push(subTask);
      }
      // Otherwise, if it is a parent task, add its executors to the list of tasks.
      else if (subTask instanceof ParentTask) {
        tasks.push(subTask.allTasks);
      }
    });

    return tasks.flat();
  }

  /**
   * Creates a new `ParentTask` instance.
   * @inheritdoc
   * @param constructorTask - The task constructor that defines the task.
   * @param [parent] - The parent task of this task. If none is passed this task is
   * assumed to be a root task.
   */
  public constructor(
    constructorTask: IParentTaskConstructor,
    parent?: IParentTask
  ) {
    makeObservable(this);
    this.name = constructorTask.name;
    this.parent = parent ?? null;
    this.subTasks = constructorTask.subTasks.map((task) => {
      // If the task is a parent task, create a new parent task.
      if ('subTasks' in task) {
        return new ParentTask(task, this);
      }
      // If the task is an executable task, create a new executable task.
      else {
        return new ExecutableTask(task, this);
      }
    });
  }
}
