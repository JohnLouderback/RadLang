using System.Diagnostics;
using DryIoc;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using RadLanguageServerV2.Handlers;
using RadLanguageServerV2.Services;

namespace RadLanguageServerV2;

/// <summary>
///   The <c> LanguageServer </c> interoperates between requests to the language server and its handler implementations.
/// </summary>
public class LanguageServer {
  private const string handlerNotFoundMessage =
    "Could not find a matching request handler for the given type parameters.";

  private readonly RequestHandlerDictionary handlers = new();
  private readonly IContainer container = new Container();
  private LanguageRPCServer? languageRPCServer;

  /// <summary>
  ///   Whether the language server's <see cref="Start" /> method has been called and the language server
  ///   is running.
  /// </summary>
  public bool Initialized { get; private set; }


  public LanguageServer() {
    container.RegisterInstance(new LanguageServerService(this));
  }


  /// <summary>
  ///   Gets a request handler for a given parameter and return type. If one exists.
  /// </summary>
  /// <typeparam name="T"> Represents the type of the request handler's arguments. </typeparam>
  /// <typeparam name="TReturn"> Represents the type of the request handler's return value. </typeparam>
  /// <returns> The requested request handler function. </returns>
  /// <exception cref="InvalidOperationException"> Will be thrown if a request handler could not be found. </exception>
  public RequestHandler<T, TReturn> GetHandler
    <T, TReturn>(string endpoint) where T : class {
    // Check that the dictionary contains an entry for this type of request handler.
    if (!handlers.ContainsKey(endpoint)) {
      throw new InvalidOperationException(
          handlerNotFoundMessage
        );
    }

    // Attempt to get the found request handler class.
    var handler = handlers[endpoint] as Type ??
                  throw new InvalidOperationException(
                      handlerNotFoundMessage
                    );

    // Attempt to get the singleton request handler instance from the DI container.
    return (container.Resolve(handler) as IRequestHandler<T, TReturn> ??
            throw new InvalidOperationException(
                handlerNotFoundMessage
              )).Handler;
  }


  /// <summary>
  ///   Gets a request handler for a given parameter and return type. If one exists.
  /// </summary>
  /// <typeparam name="T"> Represents the type of the request handler's arguments. </typeparam>
  /// <returns> The requested request handler function. </returns>
  /// <exception cref="InvalidOperationException"> Will be thrown if a request handler could not be found. </exception>
  public VoidRequestHandler<T> GetHandler
    <T>(string endpoint) where T : class {
    // Check that the dictionary contains an entry for this type of request handler.
    if (!handlers.ContainsKey(endpoint)) {
      throw new InvalidOperationException(
          handlerNotFoundMessage
        );
    }

    // Attempt to get the found request handler class.
    var handler = handlers[endpoint] as Type ??
                  throw new InvalidOperationException(
                      handlerNotFoundMessage
                    );

    // Attempt to get the singleton request handler instance from the DI container.
    return (container.Resolve(handler) as IVoidRequestHandler<T> ??
            throw new InvalidOperationException(
                handlerNotFoundMessage
              )).Handler;
  }


  public async Task PublishDiagnostics(PublishDiagnosticParams parameter) {
    await languageRPCServer.SendMethodNotificationAsync(
        new LspNotification<PublishDiagnosticParams>(Methods.TextDocumentPublishDiagnosticsName),
        parameter
      );
  }


  /// <summary>
  ///   Initializes and starts the language server.
  /// </summary>
  /// <returns> This instance of the <see cref="LanguageServer" />. </returns>
  public LanguageServer Start(Stream input, Stream output) {
    // Set up the tracing logging.
    var traceSource = new TraceSource(
        "Rad Language Server",
        SourceLevels.Verbose | SourceLevels.ActivityTracing
      );
    // Store the logs in the system temporary directory.
    var traceFileDirectoryPath = Path.Combine(Path.GetTempPath(), "RadLang", "LSP");
    Directory.CreateDirectory(traceFileDirectoryPath);
    var logFilePath   = Path.Combine(traceFileDirectoryPath, "log.svclog");
    var traceListener = new XmlWriterTraceListener(logFilePath);
    traceSource.Listeners.Add(traceListener);
    Trace.AutoFlush = true;

    // Set up the RPC server for handling the JSON RPC requests.
    languageRPCServer = new LanguageRPCServer(
        this,
        traceSource,
        output,
        input
      );

    Initialized = true;

    return this;
  }


  /// <summary>
  ///   Registers a language server request handler with the given argument and returns types for handling requests of those
  ///   types.
  /// </summary>
  /// <typeparam name="T"> Represents the type of the request handler's arguments. </typeparam>
  /// <typeparam name="TReturn"> Represents the type of the request handler's return value. </typeparam>
  /// <typeparam name="THandler"> The type of the request handler function. </typeparam>
  /// <returns> This instance of the <see cref="LanguageServer" />. </returns>
  public LanguageServer WithHandler
    <T, TReturn, THandler>(string endpoint)
    where T : class where THandler : IRequestHandler<T, TReturn> {
    container.Register<THandler>(Reuse.Singleton);
    handlers[endpoint] = typeof(THandler);
    return this;
  }


  /// <summary>
  ///   Registers a language server request handler with the given argument type for handling requests of that
  ///   type.
  /// </summary>
  /// <typeparam name="T"> Represents the type of the request handler's arguments. </typeparam>
  /// <typeparam name="THandler"> The type of the request handler function. </typeparam>
  /// <returns> This instance of the <see cref="LanguageServer" />. </returns>
  public LanguageServer WithHandler
    <T, THandler>(string endpoint) where T : class where THandler : IVoidRequestHandler<T> {
    container.Register<THandler>(Reuse.Singleton);
    handlers[endpoint] = typeof(THandler);
    return this;
  }


  public LanguageServer WithService<TService>() {
    container.Register<TService>(Reuse.Singleton);
    return this;
  }
}
