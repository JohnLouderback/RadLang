using System.Runtime.Serialization;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Newtonsoft.Json;

namespace RadLanguageServerV2.LanguageServerEx.Models.InlayHint;

/// <summary>
///   Inlay hint information.
///   @since 3.17.0
/// </summary>
[DataContract]
public class InlayHint {
	/// <summary>
	///   The position of this hint.
	/// </summary>
	/// <since> 3.17.0 </since>
	[DataMember(Name = "position")]
  public Position Position { get; set; }

	/// <summary>
	///   The label of this hint. A human readable string or an array of
	///   InlayHintLabelPart label parts.
	///   *Note* that neither the string nor the label part can be empty.
	/// </summary>
	[DataMember(Name = "label")]
  public SumType<string, InlayHintLabelPart> Label { get; set; }

	/// <summary>
	///   The kind of this hint. Can be omitted in which case the client
	///   should fall back to a reasonable default.
	/// </summary>
	[DataMember(Name = "kind")]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  public InlayHintKind? Kind { get; set; }

	/// <summary>
	///   Optional text edits that are performed when accepting this inlay hint.
	///   *Note* that edits are expected to change the document so that the inlay
	///   hint (or its nearest variant) is now part of the document and the inlay
	///   hint itself is now obsolete.
	///   Depending on the client capability `inlayHint.resolveSupport` clients
	///   might resolve this property late using the resolve request.
	/// </summary>
	[DataMember(Name = "textEdits")]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  public TextEdit[] TextEdits { get; set; }

	/// <summary>
	///   The tooltip text when you hover over this item.
	///   Depending on the client capability `inlayHint.resolveSupport` clients
	///   might resolve this property late using the resolve request.
	/// </summary>
	[DataMember(Name = "tooltip")]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  public SumType<string, MarkupContent>? Tooltip { get; set; }

	/// <summary>
	///   Render padding before the hint.
	///   Note: Padding should use the editor's background color, not the
	///   background color of the hint itself. That means padding can be used
	///   to visually align/separate an inlay hint.
	/// </summary>
	[DataMember(Name = "paddingLeft")]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  public bool? PaddingLeft { get; set; }

	/// <summary>
	///   Render padding after the hint.
	///   Note: Padding should use the editor's background color, not the
	///   background color of the hint itself. That means padding can be used
	///   to visually align/separate an inlay hint.
	/// </summary>
	[DataMember(Name = "paddingRight")]
  [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
  public bool? PaddingRight { get; set; }

  // /// <summary>
  // /// A data entry field that is preserved on a inlay hint between
  // /// a `textDocument/inlayHint` and a `inlayHint/resolve` request.
  // /// </summary>
  // [DataMember(Name = "data")]
  // public LSPAny Data { get; set; }
}
