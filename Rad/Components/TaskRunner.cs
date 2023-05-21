using Spectre.Console;

namespace Rad.Components;

/// <summary>
///   A simple task runner that uses <c> Spectre.Console </c>  to display a spinner and a task name.
/// </summary>
public static class TaskRunner {
  private static readonly Style spinnerStyle = Style.Parse("green");
  private static readonly Spinner spinner = Spinner.Known.Arrow3;


  /// <summary>
  ///   Runs a task with a spinner and a task name.
  /// </summary>
  /// <param name="taskName">
  ///   The name of the task. This text is displayed next to the spinner. It provides the end-user
  ///   with a hint about what the task is doing.
  /// </param>
  /// <param name="action">
  ///   The task to run. This is the code that does the work to complete the task at hand.
  /// </param>
  /// <example>
  ///   <code>
  ///   TaskRunner.Run("Doing something", context => {
  ///     // Do something here.
  ///   });
  ///   </code>
  /// </example>
  public static void Run(string taskName, Action<StatusContext> action) {
    AnsiConsole.Status()
      .Spinner(spinner)
      .SpinnerStyle(spinnerStyle)
      .Start(taskName, action);
  }


  /// <inheritdoc cref="Run(string,System.Action{Spectre.Console.StatusContext})" />
  public static void Run(string taskName, Func<StatusContext, Task> action) {
    AnsiConsole.Status()
      .Spinner(spinner)
      .SpinnerStyle(spinnerStyle)
      .Start(
          taskName,
          async ctx => {
            await action(ctx);
            ctx.Refresh();
          }
        );
  }


  /// <inheritdoc cref="Run(string,System.Action{Spectre.Console.StatusContext})" />
  public static T Run<T>(string taskName, Func<StatusContext, T> action) where T : Task {
    return AnsiConsole.Status()
      .Spinner(spinner)
      .SpinnerStyle(spinnerStyle)
      .Start(
          taskName,
          action
        );
  }
}
