using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Rad.Commands;
using Spectre.Console;
using Spectre.Console.Cli;

var app = new CommandApp();

app.Configure(config =>
{
  config.AddCommand<RunCommand>("run")
    .WithAlias("r")
    .WithDescription("Executes a provided file.");
});

return app.Run(args);
