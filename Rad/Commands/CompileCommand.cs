using Rad.Components;
using Rad.Toolchains;
using Rad.Utils;
using RadCompiler;
using RadCompiler.Utils;
using RadProject;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Rad.Commands;

public class CompileCommand : AsyncCommand<CompileCommand.Settings> {
  public override async Task<int> ExecuteAsync(CommandContext context, Settings settings) {
    LogoPrinter.Print();

    var panel = new FilePanel(settings.File);

    var         status     = 0;
    LLVMResult? llvmResult = default;

    var executable =
      await TaskRunner.Run(
          "Running...",
          async ctx => {
            AnsiConsole.Write(panel);
            try {
              IExecutable executable = new WasmFile();
              executable.BuildStatus += (sender, args) => {
                AnsiConsole.WriteLine(args.Message + " " + args.Details);
              };

              executable.Build(Project.FromEntryPoint(settings.File));

              return executable;
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

    if (llvmResult is not null) {
      Logging.LLVMResult(llvmResult);
    }

    // If the script is null, then we encountered an error while building the script. We should
    // return the status code of the error.
    if (executable is null) {
      return status;
    }

    // Post-build steps. This includes utilizing any other compiler toolchains for building. This is
    // used when the executable is not a native executable, but rather a WasmFile or something else.
    // Generally, this is where another toolchain would take over and continue the build process.

    // If the executable is a WasmFile, then we should use the Emscripten toolchain to
    // pick up the build.
    if (executable is WasmFile wasmFile) {
      var emscripten = ToolchainFactory.GetToolchain<EmscriptenToolchain>();
      await emscripten.Activate();
      await emscripten.HandOffBuild(
          wasmFile.IRFilePath,
          _ => { Logging.Success("It has been written to the file system."); }
        );
    }

    return status;
  }


  public class Settings : CommandSettings {
    [CommandArgument(0, "[Name]")] public string File { get; set; }
  }
}
