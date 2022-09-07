using OmniSharp.Extensions.LanguageServer.Protocol;
using RadLanguageServer.Constructs;

namespace RadLanguageServer.Services;

/// <summary>
///   Service for managing open documents.
/// </summary>
public class DocumentManagerService {
  /// <summary>
  ///   Dictionary of open documents, keyed by URI.
  /// </summary>
  public Dictionary<DocumentUri, DocumentContent> Documents { get; } = new();
}
