using Rad.Components;
using Rad.Utils;
using RadCompiler;
using RadCompiler.Utils;
using RadProject;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Rad.Commands;

public class RunCommand : AsyncCommand<RunCommand.Settings> {
  public override async Task<int> ExecuteAsync(CommandContext context, Settings settings) {
    LogoPrinter.Print();

    var panel = new FilePanel(settings.File);

    var         status     = 0;
    LLVMResult? llvmResult = default;

    // Build the script is a separate thread so that we can display the build status in the console
    // then return the script to the main thread to execute in.
    var script =
      await TaskRunner.Run(
          "Running...",
          async ctx => {
            AnsiConsole.Write(panel);
            try {
              var script = new Script();
              script.BuildStatus += (sender, args) => {
                AnsiConsole.WriteLine(args.Message + " " + args.Details);
              };
              script.Build(Project.FromEntryPoint(settings.File));
              return script;
            }
            catch (Exception e) {
              if (e is LLVMResult result) {
                llvmResult = result;
                status     = llvmResult.ResultType == LLVMResultType.Error ? -1 : 0;
                return null;
              }

              AnsiConsole.WriteException(
                  e,
                  ExceptionFormats.ShortenMethods | ExceptionFormats.ShowLinks
                );
              status = -1;
              return null;
            }
          }
        );

    // If we have an LLVM result, then we should log it. This may indicate either issues with the
    // script or issues with the compiler.
    if (llvmResult is not null) {
      Logging.LLVMResult(llvmResult);
    }

    // If the script is null, then we encountered an error while building the script. We should
    // return the status code of the error.
    if (script is null) {
      return status;
    }

    // If the script is not null, then we can run it now. The script will output to stdout.
    script.Run();

    return status;
  }


  public class Settings : CommandSettings {
    [CommandArgument(0, "[Name]")] public string File { get; set; }
  }
}
