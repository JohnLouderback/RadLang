namespace RadLanguageServerV2.Constructs;

/// <summary>
///   Represents a semantic token modifier as defined by the LSP specification.
///   <see
///     href="https://microsoft.github.io/language-server-protocol/specifications/specification-current/#textDocument_semanticTokens" />
/// </summary>
[Flags]
public enum SemanticTokenModifier {
  Declaration = 1,
  Definition = 2,
  Readonly = 4,
  Static = 8,
  Deprecated = 16,
  Abstract = 32,
  Async = 64,
  Modification = 128,
  Documentation = 256,
  DefaultLibrary = 512
}
