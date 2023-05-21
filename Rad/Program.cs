using Rad.Commands;
using Spectre.Console;
using Spectre.Console.Cli;

AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
  AnsiConsole.WriteException(e.ExceptionObject as Exception, ExceptionFormats.ShortenEverything);
};

var app = new CommandApp();

app.Configure(
    config => {
      config.AddCommand<RunCommand>("run")
        .WithAlias("r")
        .WithDescription("Executes a provided file as a script.");
      config.AddCommand<CompileCommand>("compile")
        .WithAlias("c")
        .WithDescription("Takes a project or source file and builds it into an executable.");
      config.AddCommand<ToolchainCommand>("toolchain");
    }
  );

return app.Run(args);
