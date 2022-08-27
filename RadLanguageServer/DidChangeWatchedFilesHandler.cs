using MediatR;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Workspace;

namespace RadLanguageServer;

internal class DidChangeWatchedFilesHandler : IDidChangeWatchedFilesHandler {
  public DidChangeWatchedFilesRegistrationOptions GetRegistrationOptions() {
    return new DidChangeWatchedFilesRegistrationOptions();
  }


  public DidChangeWatchedFilesRegistrationOptions GetRegistrationOptions(
    DidChangeWatchedFilesCapability capability,
    ClientCapabilities clientCapabilities
  ) {
    return new DidChangeWatchedFilesRegistrationOptions();
  }


  public Task<Unit> Handle(
    DidChangeWatchedFilesParams request,
    CancellationToken cancellationToken
  ) {
    return Unit.Task;
  }
}
