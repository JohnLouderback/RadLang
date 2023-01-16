using System.Runtime.Serialization;

namespace RadLanguageServerV2.LanguageServerEx.Models.InlayHint;

/// <summary>
///   Inlay hint kinds.
/// </summary>
/// <since> 3.17.0 </since>
[DataContract]
public enum InlayHintKind {
  /// <summary>
  ///   An inlay hint that for a type annotation.
  /// </summary>
  [EnumMember]
  Type = 1,

  /// <summary>
  ///   An inlay hint that is for a parameter.
  /// </summary>
  [EnumMember]
  Parameter = 2
}
