using RadCompiler.Utils;
using RadInterpreter;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Rad.Commands;

public class RunCommand : Command<RunCommand.Settings> {
  public override int Execute(CommandContext context, Settings settings) {
    var path = new TextPath(Emoji.Replace(":page_facing_up: ") + settings.File)
      .LeafColor(Color.Blue)
      .StemStyle(new Style(null, null, Decoration.Dim))
      .RootStyle(new Style(null, null, Decoration.Dim))
      .SeparatorStyle(new Style(null, null, Decoration.Dim));

    var panel = new Panel(path) {
      Header = new PanelHeader("Compiling File:", Justify.Left),
      Border = BoxBorder.Rounded,
      Expand = false
    };
    panel.BorderColor(Color.Blue);

    var         status     = 0;
    LLVMResult? llvmResult = default;

    AnsiConsole.Status()
      .Spinner(Spinner.Known.Arrow3)
      .SpinnerStyle(Style.Parse("green"))
      .Start(
          "Running...",
          async ctx => {
            AnsiConsole.Write(panel);
            try {
              var interpreter = new Interpreter();
              interpreter.Status += (sender, args) => {
                AnsiConsole.WriteLine(args.Message + " " + args.Details);
              };
              interpreter.InterpretFile(settings.File);
            }
            catch (Exception e) {
              if (e is LLVMResult result) {
                llvmResult = result;
                status     = llvmResult.ResultType == LLVMResultType.Error ? -1 : 0;
                return;
              }

              AnsiConsole.WriteException(
                  e,
                  ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks
                );
              status = -1;
              return;
            }

            ctx.Refresh();
          }
        );

    if (llvmResult is not null) {
      var originalColor = Console.ForegroundColor;
      var header = llvmResult.ResultType == LLVMResultType.Error
                     ? new Rule("[Red]Internal Compiler Error [/](╯’□’)╯︵ ┻━┻")
                     : new Rule(":partying_face:[Green] Success [/]:party_popper:");
      header.Alignment = Justify.Left;
      header.Border    = BoxBorder.Double;
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
              Alignment = Justify.Left,
              Border    = BoxBorder.Double
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

    return status;
  }


  public class Settings : CommandSettings {
    [CommandArgument(0, "[Name]")] public string File { get; set; }
  }
}
