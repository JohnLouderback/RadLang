using System.Runtime.Serialization;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Range = Microsoft.VisualStudio.LanguageServer.Protocol.Range;

namespace RadLanguageServerV2.LanguageServerEx.Params.InlayHint;

/// <summary>
///   A parameter literal used in inlay hint requests.
///   <see
///     href="https://microsoft.github.io/language-server-protocol/specifications/lsp/3.17/specification/#inlayHintParams" />
/// </summary>
/// <since> 3.17.0 </since>
[DataContract]
public class InlayHintParams {
  /// <summary>
  ///   The text document.
  /// </summary>
  [DataMember(Name = "textDocument")]
  public TextDocumentIdentifier TextDocument;

  /// <summary>
  ///   The visible document range for which inlay hints should be computed.
  /// </summary>
  [DataMember(Name = "range")]
  public Range Range;
}
