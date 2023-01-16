using System.Runtime.Serialization;
using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace RadLanguageServerV2.LanguageServerEx.Options;

/// <inheritdoc />
/// <summary>
///   A class used to override the <see cref="T:Microsoft.VisualStudio.LanguageServer.Protocol.SemanticTokensLegend" /> as
///   it is not annotated
///   correctly with the <c> [DataContract] </c> annotation in the base library.
/// </summary>
[DataContract]
public class SemanticTokensLegendOptions : SemanticTokensLegend {
  /// <summary>
  ///   Gets or sets an array of token types that can be encoded in semantic tokens responses.
  /// </summary>
  [DataMember(Name = "tokenTypes")]
  public new string[] TokenTypes { get; set; }

  /// <summary>
  ///   Gets or sets an array of token modfiers that can be encoded in semantic tokens responses.
  /// </summary>
  [DataMember(Name = "tokenModifiers")]
  public new string[] TokenModifiers { get; set; }
}
