namespace RadLanguageServerV2.Handlers;

/// <summary>
///   A delegate representing a handler for language server requests.
/// </summary>
/// <typeparam name="T"> Represents the type of the request handler's arguments. </typeparam>
/// <typeparam name="TReturn"> Represents the type of the request handler's return value. </typeparam>
public delegate Task<TReturn> RequestHandler<T, TReturn>(T args) where T : class;

public interface IRequestHandler<T, TReturn> where T : class {
  Task<TReturn> Handler(T args);
}
