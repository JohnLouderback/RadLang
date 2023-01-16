using System.Runtime.Serialization;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Newtonsoft.Json;

namespace RadLanguageServerV2.LanguageServerEx.Models.InlayHint;

/// <summary>
///   An inlay hint label part allows for interactive and composite labels
///   of inlay hints.
/// </summary>
/// <since> 3.17.0 </since>
[DataContract]
public class InlayHintLabelPart {
  /// <summary>
  ///   The value of this label part.
  /// </summary>
  [DataMember(Name = "value")]
  public string Value { get; set; }

  /// <summary>
  ///   The tooltip text when you hover over this label part. Depending on
  ///   the client capability `inlayHint.resolveSupport` clients might resolve
  ///   this property late using the resolve request.
  /// </summary>
  [DataMember(Name = "tooltip")]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  public SumType<string, MarkupContent>? Tooltip { get; set; }

  /// <summary>
  ///   An optional source code location that represents this
  ///   label part.
  ///   The editor will use this location for the hover and for code navigation
  ///   features: This part will become a clickable link that resolves to the
  ///   definition of the symbol at the given location (not necessarily the
  ///   location itself), it shows the hover that shows at the given location,
  ///   and it shows a context menu with further code navigation commands.
  ///   Depending on the client capability `inlayHint.resolveSupport` clients
  ///   might resolve this property late using the resolve request.
  /// </summary>
  [DataMember(Name = "location")]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  public Location? Location { get; set; }

  /// <summary>
  ///   An optional command for this label part.
  ///   Depending on the client capability `inlayHint.resolveSupport` clients
  ///   might resolve this property late using the resolve request.
  /// </summary>
  [DataMember(Name = "command")]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  public Command? Command { get; set; }
}
