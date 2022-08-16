using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Rad.Commands;
using Spectre.Console;
using Spectre.Console.Cli;

AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler((object sender, UnhandledExceptionEventArgs e) => {
  AnsiConsole.WriteException(e.ExceptionObject as Exception, ExceptionFormats.ShortenEverything);
});

var app = new CommandApp();

app.Configure(config =>
{
  config.AddCommand<RunCommand>("run")
    .WithAlias("r")
    .WithDescription("Executes a provided file.");
});

return app.Run(args);
