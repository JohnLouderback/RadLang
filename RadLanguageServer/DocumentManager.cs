using OmniSharp.Extensions.LanguageServer.Protocol;

namespace RadLanguageServer;

public class DocumentManager {
  public Dictionary<DocumentUri, DocumentContent> Documents { get; } = new();
}
