using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace RadLanguageServerV2.Services;

public class LanguageServerService {
  private readonly LanguageServer languageServer;


  public LanguageServerService(LanguageServer languageServer) {
    this.languageServer = languageServer;
  }


  public async Task PublishDiagnostics(PublishDiagnosticParams parameter) {
    await languageServer.PublishDiagnostics(parameter);
  }
}
