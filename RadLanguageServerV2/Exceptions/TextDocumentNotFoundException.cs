namespace RadLanguageServerV2.Exceptions;

public class TextDocumentNotFoundException : KeyNotFoundException {
  public TextDocumentNotFoundException(Uri textDocumentUri) : base(
      $"Text document not found for URI \"{textDocumentUri}\"."
    ) {}
}
