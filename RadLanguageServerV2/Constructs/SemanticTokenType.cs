namespace RadLanguageServerV2.Constructs;

/// <summary>
///   Specifies the type of a semantic token in the language server.
/// </summary>
/// <see href="https://code.visualstudio.com/api/language-extensions/semantic-highlight-guide" />
public enum SemanticTokenType {
  Namespace,
  Class,
  Enum,
  Interface,
  Struct,
  TypeParameter,
  Type,
  Parameter,
  Variable,
  Property,
  EnumMember,
  Decorator,
  Event,
  Function,
  Method,
  Macro,
  Label,
  Comment,
  String,
  Keyword,
  Number,
  Regexp,
  Operator,
  Modifier
}
