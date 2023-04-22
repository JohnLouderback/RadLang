namespace RadLanguageServerV2.Exceptions;

/// <summary>
///   Exception thrown when a text document is not found in the document manager.
/// </summary>
public class TextDocumentNotFoundException : KeyNotFoundException {
  public TextDocumentNotFoundException(Uri textDocumentUri) : base(
      $"Text document not found for URI \"{textDocumentUri}\"."
    ) {}
}
