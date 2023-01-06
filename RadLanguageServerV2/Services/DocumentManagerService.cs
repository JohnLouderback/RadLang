using RadLanguageServerV2.Constructs;

namespace RadLanguageServerV2.Services;

/// <summary>
///   Service for managing open documents.
/// </summary>
public class DocumentManagerService {
  /// <summary>
  ///   Dictionary of open documents, keyed by URI.
  /// </summary>
  public Dictionary<Uri, DocumentContent> Documents { get; } = new();
}
