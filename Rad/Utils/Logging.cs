using RadCompiler.Utils;
using Spectre.Console;

namespace Rad.Utils;

/// <summary>
///   This class is used to house the logging functions for the compiler and to abstract over
///   common logging functions and styling.
/// </summary>
public static class Logging {
  /// <summary>
  ///   Logs a message to the console at the <c> Error </c> level with the correct styling.
  /// </summary>
  /// <param name="message"> The message to log to the console. </param>
  public static void Error(string message) {
    AnsiConsole.MarkupLine($"[Red]Error [/]:loudly_crying_face: {message}");
  }


  /// <summary>
  ///   Logs a message to the console at the <c> Info </c> level with the correct styling.
  /// </summary>
  /// <param name="message"> The message to log to the console. </param>
  public static void Info(string message) {
    AnsiConsole.MarkupLine($"[Blue]Info [/]:information: {message}");
  }


  public static void LLVMResult(LLVMResult llvmResult) {
    var originalColor = Console.ForegroundColor;
    var header = llvmResult.ResultType == LLVMResultType.Error
                   ? new Rule("[Red]Internal Compiler Error [/](╯’□’)╯︵ ┻━┻")
                   : new Rule(":partying_face:[Green] Success [/]:party_popper:");
    header.Justification = Justify.Left;
    header.Border        = BoxBorder.Double;
    AnsiConsole.Write(header);
    Console.ForegroundColor = llvmResult.ResultType == LLVMResultType.Error
                                ? ConsoleColor.Red
                                : ConsoleColor.Green;
    llvmResult.GetMessage();

    // If there are any specifics, dump them now.
    if (llvmResult.GetDetails is not null) {
      Console.ForegroundColor = originalColor;
      AnsiConsole.Write(
          new Rule("[Blue]Details [/]:thinking_face:") {
            Justification = Justify.Left,
            Border        = BoxBorder.Double
          }
        );
      Console.WriteLine("");
      Console.ForegroundColor = ConsoleColor.Blue;
      llvmResult.GetDetails();
      Console.WriteLine("");
    }

    Console.ForegroundColor = originalColor;
    AnsiConsole.Write(
        new Rule {
          Border = BoxBorder.Double
        }
      );
    Console.WriteLine("");
  }


  /// <summary>
  ///   Logs a message to the console with the correct styling to denote the success of an
  ///   operation.
  /// </summary>
  /// <param name="message">
  ///   The message to log to the console. Generally, what was successful or what the successful
  ///   result was.
  /// </param>
  public static void Success(string message) {
    AnsiConsole.MarkupLine($"[Green]Success [/]:partying_face:: {message}");
  }
}
