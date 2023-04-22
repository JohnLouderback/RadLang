using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace RadLanguageServerV2.Services;

/// <summary>
///   The <c> LanguageServerService </c> class is responsible for providing access to the language server
///   from other services. It is a facade for the language server.
/// </summary>
public class LanguageServerService {
  private readonly LanguageServer languageServer;


  public LanguageServerService(LanguageServer languageServer) {
    this.languageServer = languageServer;
  }


  public async Task PublishDiagnostics(PublishDiagnosticParams parameter) {
    await languageServer.PublishDiagnostics(parameter);
  }
}
