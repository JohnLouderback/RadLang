using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace RadLanguageServerV2.LanguageServerEx.Options.InlayHints;

/// <summary>
///   Inlay hint options used during static registration.
/// </summary>
/// <since> 3.17.0 </since>
[DataContract]
public class InlayHintOptions {
  /// <summary>
  ///   The server provides support to resolve additional
  ///   information for an inlay hint item.
  /// </summary>
  [DataMember(Name = "resolveProvider")]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  public bool? ResolveProvider { get; set; }
}
