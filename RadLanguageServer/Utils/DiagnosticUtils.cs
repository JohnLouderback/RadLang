using RadDiagnostics;
using LSPSeverity = OmniSharp.Extensions.LanguageServer.Protocol.Models.DiagnosticSeverity;

namespace RadLanguageServer.Utils;

public static class DiagnosticUtils {
  /// <summary>
  ///   Converts a Rad <see cref="DiagnosticSeverity" /> to a LSP <c> DiagnosticSeverity </c>
  ///   representation.
  /// </summary>
  /// <param name="severity"> </param>
  /// <returns> </returns>
  public static LSPSeverity ToLSPSeverity(DiagnosticSeverity severity) {
    return severity switch {
      DiagnosticSeverity.Error       => LSPSeverity.Error,
      DiagnosticSeverity.Warning     => LSPSeverity.Warning,
      DiagnosticSeverity.Information => LSPSeverity.Information,
      DiagnosticSeverity.Hint        => LSPSeverity.Hint
    };
  }
}
