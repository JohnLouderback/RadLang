import { action, computed, makeObservable, observable } from 'mobx';

import { Nullable } from '../type-utils.js';
import {
  IExecutableTask,
  IExecutableTaskConstructor,
  IParentTask,
  TaskStatus
} from './ITask.js';

/**
 * Represents a task that executes some action.
 */
export class ExecutableTask implements IExecutableTask {
  private _shouldSkip: Nullable<(log: (message: string) => void) => boolean>;

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

  @observable private _progress: number = 0;
  @observable private _status: TaskStatus = TaskStatus.Pending;

  /** @inheritdoc */
  public readonly name: string;

  /** @inheritdoc */
  @computed public get progress(): number {
    return this._progress;
  }
  @computed public get status(): TaskStatus {
    return this._status;
  }

  /** @inheritdoc */
  @observable public readonly parent: Nullable<IParentTask>;

  /** @inheritdoc */
  public readonly executor: (
    setProgress: (progress: number) => void,
    log: (message: string) => void
  ) => Promise<void>;

  /**
   * Creates a new `ExecutableTask` instance.
   * @inheritdoc
   * @param constructorTask - The task constructor that defines the task.
   * @param [parent] - The parent task of this task. If none is passed, this task is
   * assumed to be a root task.
   */
  public constructor(
    constructorTask: IExecutableTaskConstructor,
    parent?: IParentTask
  ) {
    makeObservable(this);
    this._shouldSkip = constructorTask.shouldSkip ?? null;
    this.name = constructorTask.name;
    this.parent = parent ?? null;
    this._log = [...(constructorTask.log ?? [])];
    this.executor = constructorTask.executor;
  }

  /**
   * Executes this task, running its executor function.
   */
  @action public async execute(): Promise<void> {
    if (this.shouldSkip()) {
      this.skip();
      return;
    }
    this._status = TaskStatus.InProgress;
    this.log.push(this.name);
    await this.executor(
      this.setProgress.bind(this),
      this.logMessage.bind(this)
    );
    this._status = TaskStatus.Completed;
  }

  private setProgress(progress: number): void {
    progress = Math.round(progress);
    this._progress = progress;
  }

  private logMessage(message: string): void {
    this.log.push(message);
  }

  /** @inheritdoc */
  shouldSkip(): boolean {
    if (this._shouldSkip) {
      return this._shouldSkip(this.logMessage.bind(this));
    }
    return false;
  }

  /** @inheritdoc */
  @action public skip() {
    this._status = TaskStatus.Skipped;
    this._progress = 100;
  }
}
