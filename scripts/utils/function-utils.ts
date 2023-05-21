/**
 * Runs a list of async functions in order.
 * @param functions - The list of functions to run.
 * @param args - The arguments to pass to each function.
 * @returns A promise that resolves when all functions have been run.
 */
export const runAsyncFunctionsInOrder = <
  T extends (...args: Array<any>) => Promise<void>,
  Context
>(
  functions: Array<[T, Context]>,
  argsFactory?: (context: Context) => Readonly<Parameters<T>>
) => {
  // Create an empty promise to start the chain.
  let promise = Promise.resolve();

  // Loop through each function and chain it to the previous one.
  functions.forEach((fn) => {
    const [func, context] = fn;
    // Call the function with the arguments and chain it to the previous promise.
    promise = promise.then(() =>
      func.apply(
        context,
        // If an args factory is provided, call it to get the arguments.
        argsFactory ? argsFactory(context) : []
      )
    );
  });

  // Return the promise chain. This will resolve when all functions have been run.
  return promise;
};
