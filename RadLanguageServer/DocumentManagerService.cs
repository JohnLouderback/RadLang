using OmniSharp.Extensions.LanguageServer.Protocol;

namespace RadLanguageServer;

public class DocumentManagerService {
  public Dictionary<DocumentUri, DocumentContent> Documents { get; } = new();
}
