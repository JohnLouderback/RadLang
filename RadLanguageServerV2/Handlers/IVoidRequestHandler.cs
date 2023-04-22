namespace RadLanguageServerV2.Handlers;

/// <summary>
///   A delegate representing a handler for language server requests.
/// </summary>
/// <typeparam name="T"> Represents the type of the request handler's arguments. </typeparam>
public delegate Task VoidRequestHandler<T>(T args) where T : class;

/// <summary>
///   A request handler is responsible for handling requests from the language server. Each request handler
///   has a <see cref="IVoidRequestHandler{T}.Handler" /> method that is called when the request is received.
///   This is a void request handler, meaning that it does not return a value which differs from the
///   <see cref="IRequestHandler{T,TReturn}" /> type which does return a value from the handler method.
/// </summary>
/// <typeparam name="T"> The type of the request handler's arguments. </typeparam>
public interface IVoidRequestHandler<T> where T : class {
  /// <summary>
  ///   The handler method for the request. This method is called when the request is received and is
  ///   passed the request arguments for handling this specific request.
  /// </summary>
  /// <param name="args"> arguments of the request of the type specified by <typeparamref name="T" /> </param>
  /// <returns> a <see cref="Task" /> that completes when the request has been handled. </returns>
  Task Handler(T args);
}
