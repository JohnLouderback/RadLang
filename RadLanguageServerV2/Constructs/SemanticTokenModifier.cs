namespace RadLanguageServerV2.Constructs;

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
