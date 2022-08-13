using RadInterpreter;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Rad.Commands; 

public class RunCommand : Command<RunCommand.Settings> {
  public class Settings : CommandSettings
  {
    [CommandArgument(0, "[Name]")]
    public string File { get; set; }
  }
  
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

    AnsiConsole.Status()
      .Spinner(Spinner.Known.Arrow3)
      .SpinnerStyle(Style.Parse("green bold"))
      .Start($"Running..." , async ctx => {
        AnsiConsole.Write(panel);
        (new Interpreter()).InterpretFile(settings.File);
        ctx.Refresh();
      });
    return 0;
  }
}
