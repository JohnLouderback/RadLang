using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace RadLanguageServerV2.LanguageServerEx;

/// <summary>
///   Provides RPC method endpoints for the JSON RPC LSP server.
///   Similar to the <see cref="Methods" /> class, however it adds methods missing from that class.
///   This class does not extend <see href="Methods" /> as it is sealed.
/// </summary>
public class MethodsEx {
  /// <summary> Method name for 'textDocument/inlayHint' </summary>
  public const string InlayHintsName = "textDocument/inlayHint";

  /// <summary> Strongly typed message object for 'textDocument/inlayHint' </summary>
  public static readonly LspRequest<CodeLens, CodeLens> InlayHints = new("codeLens/resolve");
}
