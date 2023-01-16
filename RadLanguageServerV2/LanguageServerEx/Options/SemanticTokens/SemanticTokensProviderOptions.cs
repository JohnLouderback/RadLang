using System.Runtime.Serialization;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Newtonsoft.Json;

namespace RadLanguageServerV2.LanguageServerEx.Options;

/// <inheritdoc />
/// <summary>
///   A class used to override the <see cref="T:Microsoft.VisualStudio.LanguageServer.Protocol.SemanticTokensOptions" /> as
///   it is not annotated
///   correctly with the <c> [DataContract] </c> annotation in the base library.
/// </summary>
[DataContract]
public class SemanticTokensProviderOptions : SemanticTokensOptions {
  /// <summary>
  ///   Gets or sets a legend describing how semantic token types and modifiers are encoded in responses.
  /// </summary>
  [DataMember(Name = "legend")]
  public new SemanticTokensLegendOptions Legend { get; set; }

  /// <summary>
  ///   Gets or sets a value indicating whether semantic tokens Range provider requests are supported.
  /// </summary>
  [DataMember(Name = "range")]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  public new SumType<bool, object>? Range { get; set; }

  /// <summary>
  ///   Gets or sets whether or not the server supports providing semantic tokens for a full document.
  /// </summary>
  [DataMember(Name = "full")]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  public new SumType<bool, SemanticTokensFullOptions>? Full { get; set; }

  /// <summary>
  ///   Gets or sets a value indicating whether work done progress is supported.
  /// </summary>
  [DataMember(Name = "workDoneProgress")]
  [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
  public new bool WorkDoneProgress { get; set; }
}
