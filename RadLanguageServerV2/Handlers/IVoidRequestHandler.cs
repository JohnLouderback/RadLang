namespace RadLanguageServerV2.Handlers;

/// <summary>
///   A delegate representing a handler for language server requests.
/// </summary>
/// <typeparam name="T"> Represents the type of the request handler's arguments. </typeparam>
public delegate Task VoidRequestHandler<T>(T args) where T : class;

public interface IVoidRequestHandler<T> where T : class {
  Task Handler(T args);
}
