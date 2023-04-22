namespace RadLanguageServerV2.Handlers;

/// <summary>
///   A delegate representing a handler for language server requests.
/// </summary>
/// <typeparam name="T"> Represents the type of the request handler's arguments. </typeparam>
/// <typeparam name="TReturn"> Represents the type of the request handler's return value. </typeparam>
public delegate Task<TReturn> RequestHandler<T, TReturn>(T args) where T : class;

/// <summary>
///   A request handler is responsible for handling requests from the language server. Each request handler
///   has a <see cref="IRequestHandler{T,TReturn}.Handler" /> method that is called when the request is received.
/// </summary>
/// <typeparam name="T"> </typeparam>
/// <typeparam name="TReturn"> </typeparam>
public interface IRequestHandler<T, TReturn> where T : class {
  /// <summary>
  ///   The handler method for the request. This method is called when the request is received and is
  ///   passed the request arguments for handling this specific request.
  /// </summary>
  /// <param name="args"> arguments of the request of the type specified by <typeparamref name="T" /> </param>
  /// <returns> the result of the request of the type specified by <typeparamref name="TReturn" /> </returns>
  Task<TReturn> Handler(T args);
}
