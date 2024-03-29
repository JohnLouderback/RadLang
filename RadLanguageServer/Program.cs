﻿using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;
using RadLanguageServer;
using RadLanguageServer.Handlers;
using RadLanguageServer.Services;
using Serilog;

MainAsync(args).Wait();

static async Task MainAsync(string[] args) {
  Debugger.Launch();
  while (!Debugger.IsAttached) {
    await Task.Delay(100);
  }

  Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.File("log.txt")
    .MinimumLevel.Verbose()
    .CreateLogger();

  Log.Logger.Information("This only goes file...");

  IObserver<WorkDoneProgressReport> workDone = null!;

  var server =
    await LanguageServer
      .From(
          options =>
            options
              .WithInput(Console.OpenStandardInput())
              .WithOutput(Console.OpenStandardOutput())
              .ConfigureLogging(
                  x => x
                    .AddSerilog(Log.Logger)
                    .AddLanguageProtocolLogging()
                    .SetMinimumLevel(LogLevel.Debug)
                )
              .WithHandler<TextDocumentHandler>()
              .WithHandler<HoverHandler>()
              .WithHandler<DidChangeWatchedFilesHandler>()
              .WithHandler<FoldingRangeHandler>()
              .WithHandler<MyWorkspaceSymbolsHandler>()
              .WithHandler<DocumentSymbolHandler>()
              .WithHandler<SemanticTokensHandler>()
              .WithServices(x => x.AddLogging())
              .WithServices(
                  services => {
                    services.AddSingleton(
                          provider => {
                            var loggerFactory = provider.GetService<ILoggerFactory>();
                            var logger        = loggerFactory.CreateLogger<Foo>();

                            logger.LogInformation("Configuring");

                            return new Foo(logger);
                          }
                        )
                      .AddSingleton(new DocumentManagerService())
                      .AddSingleton(
                          provider => new DiagnosticsService(
                              provider.GetService<ILanguageServerFacade>(),
                              provider.GetService<DocumentManagerService>()
                            )
                        );
                  }
                )
              .OnInitialize(
                  async (server, request, token) => {
                    var manager = server.WorkDoneManager.For(
                        request,
                        new WorkDoneProgressBegin {
                          Title      = "Server is starting...",
                          Percentage = 10
                        }
                      );
                    workDone = manager;
                  }
                )
              .OnInitialized(
                  async (server, request, response, token) => {
                    workDone.OnNext(
                        new WorkDoneProgressReport {
                          Message    = "loading done",
                          Percentage = 100
                        }
                      );
                    workDone.OnCompleted();
                  }
                )
              .OnStarted(
                  async (languageServer, token) => {
                    using var manager = await languageServer.WorkDoneManager
                                          .Create(
                                              new WorkDoneProgressBegin
                                                { Title = "Doing some work..." }
                                            )
                                          .ConfigureAwait(false);

                    var logger = languageServer.Services.GetService<ILogger<Foo>>();
                    var configuration =
                      await languageServer.Configuration
                        .GetConfiguration(
                            new ConfigurationItem {
                              Section = "typescript"
                            },
                            new ConfigurationItem {
                              Section = "terminal"
                            }
                          )
                        .ConfigureAwait(false);

                    var baseConfig = new JObject();
                    foreach (var config in languageServer.Configuration.AsEnumerable()) {
                      baseConfig.Add(config.Key, config.Value);
                    }

                    logger.LogInformation("Base Config: {@Config}", baseConfig);

                    var scopedConfig = new JObject();
                    foreach (var config in configuration.AsEnumerable()) {
                      scopedConfig.Add(config.Key, config.Value);
                    }

                    logger.LogInformation("Scoped Config: {@Config}", scopedConfig);
                  }
                )
        )
      .ConfigureAwait(false);

  await server.WaitForExit.ConfigureAwait(false);
}
