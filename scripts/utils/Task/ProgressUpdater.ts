import { Tween } from '@tweenjs/tween.js';

import { Nullable } from '../type-utils.js';

/**
 * A class that can be used to animate the progress of a progress bar.
 */
export class ProgressUpdater {
  /** The current progress. */
  private progress: number;
  /** Indicates whether an animation is in progress. */
  private isAnimating: boolean = false;
  /** The tween that is used to animate the progress. */
  private tweener: Tween<{
    progress: number;
  }>;
  /** The ID of the timer that is used to update the tween. */
  private intervalID: Nullable<NodeJS.Timer> = null;
  /** The `resolve` function of the promise that is returned by `waitForAnimation`. */
  private promiseResolver: Nullable<() => void> = null;
  /** A promise that resolves when the animation is complete. */
  private promise: Promise<void> = new Promise((resolve) => {
    this.promiseResolver = resolve;
  });

  /**
   * Creates a new `ProgressUpdater` instance.
   * @inheritdoc
   * @param setProgress - A function that can be used to set the progress of the progress bar.
   * @param [initialProgress] - The initial progress. `0` by default.
   */
  public constructor(
    private setProgress: (progress: number) => void,
    initialProgress: number = 0
  ) {
    this.progress = initialProgress;
    this.tweener = new Tween({ progress: this.progress });
  }

  /**
   * Animates the progress to the specified value for the specified duration. Optionally, an easing function can be provided.
   * @param progress - The progress to animate to.
   * @param [duration] - The duration of the animation in milliseconds. `1000` by default.
   * @param [easing] - An easing function to use for the animation. For example `Easing.Quadratic.InOut`.
   */
  public animateTo(
    progress: number,
    duration: number = 1000,
    easing?: (amount: number) => number
  ): this {
    // If there is no animation timer, create one.
    if (this.intervalID === null) {
      this.intervalID = setInterval(() => {
        if (this.tweener.isPlaying()) {
          this.tweener.update();
        }
      }, 1000 / 30);
    }

    // If the animation state indicates that an animation is in progress, stop it first.
    if (this.isAnimating) {
      this.tweener.stop();
    }

    // Create a new tween to animate the progress.
    this.tweener = new Tween({ progress: this.progress })
      .to({ progress: progress }, duration)
      // On each update, set the progress both in this instance and in the progress bar.
      .onUpdate((obj) => {
        this.progress = obj.progress;
        this.setProgress(this.progress);
      })
      // On complete, set the animation state to false and clear the animation timer.
      // Finally, resolve the promise for any code that is waiting for the animation to complete.
      .onComplete(() => {
        this.isAnimating = false;
        clearInterval(this.intervalID as NodeJS.Timeout);
        this.promiseResolver?.();
      });

    // If an easing function is provided, use it.
    if (easing) {
      this.tweener.easing(easing);
    }

    // Start the tween.
    this.tweener.start();

    // Set the animation state to `true` to indicate that an animation is in progress.
    this.isAnimating = true;

    return this;
  }

  /**
   * Returns a promise that resolves when the animation is complete. Useful in
   * asynchronous functions where you wish to wait for the animation to complete before
   * continuing execution.
   * @returns A promise that resolves when the animation is complete.
   */
  public async waitForAnimation(): Promise<void> {
    // If there is no animation in progress, resolve the promise immediately.
    if (!this.isAnimating) {
      return Promise.resolve();
    }

    // Otherwise, wait for the promise to resolve.
    await this.promise;
  }
}
