import { Nullable } from '../type-utils.js';

/**
 * Represents all of the possible states of a task. Tasks are pending by default.
 */
export enum TaskStatus {
  Pending,
  InProgress,
  Completed,
  Failed
}

/**
 * Represents the shared base of all tasks.
 */
export interface ITaskBase {
  /** The name of the task. The may be any short and reasonable description. */
  name: string;

  /** The progress of the task. A number between `0` and `100`. */
  progress: number;

  /** The current state of the task. */
  status: TaskStatus;

  /** The parent task of this task. A task's parent is determined by the task that created it. */
  parent: Nullable<IParentTask>;

  /** A log of all the information pertinent to the actions this task has performed. */
  log: Array<string>;
}

/**
 * Represents a task that can be executed itself. A task that performs some action.
 */
export type IExecutableTask = ITaskBase & IExecutableTaskConstructor;

/**
 * Represents an object used to construct an executable task. This is used to declare and
 * define a task that performs some action.
 */
export interface IExecutableTaskConstructor {
  /** The name of the task. The may be any short and reasonable description. */
  name: string;

  /** A pre-populated log of all the information pertinent to the actions this task has performed. */
  log?: Array<string>;

  /**
   * The function that performs the task's action. This function is passed two functions that can be used to
   * update the task's progress and log.
   * @param setProgress - A function that can be used to set the progress of the task.
   * This should be passed a number between `0` and `100`.
   * @param log - A function that can be used to add a message to the task's log.
   */
  executor(
    setProgress: (progress: number) => void,
    log: (message: string) => void
  ): Promise<void>;
}

/**
 * Represents a task that can contain other tasks but itself does not perform any action.
 */
export type IParentTask = ITaskBase & {
  /** A list of all the subtasks of this task. These may be either executable or parent tasks. */
  subTasks: ITask[];
};

/**
 * Represents an object used to construct a parent task. This is used to declare and
 * define a task that can contain other tasks but itself does not perform any action.
 */
export interface IParentTaskConstructor {
  /** The name of the task. The may be any short and reasonable description. */
  name: string;

  /** A list of all the subtasks of this task. These may be either executable or parent tasks. */
  subTasks: ITaskConstructor[];
}

/**
 * Represents an object used to construct a task. This is used to declare and
 * define either an an {@link IExecutableTaskConstructor} or {@link IParentTaskConstructor} task.
 */
export type ITaskConstructor =
  | IExecutableTaskConstructor
  | IParentTaskConstructor;

/**
 * Represents a concrete task instance. This may be either an {@link IExecutableTask} or
 * {@link IParentTask} task.
 */
export type ITask = IParentTask | IExecutableTask;
