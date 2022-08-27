namespace RadLanguageServer;

public class DocumentContent {
  public string Text { get; private set; }
  public string[] Lines => Text.Split('\n');


  public DocumentContent(string text) {
    Text = text;
  }


  /// <summary>
  ///   Update the text to the provided string. This replaces the entire text content.
  /// </summary>
  /// <param name="text"> The text to set the document content to. </param>
  /// <returns> `this` for chaining. </returns>
  public DocumentContent Update(string text) {
    Text = text;
    return this;
  }
}
