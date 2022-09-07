using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;

namespace RadLanguageServer.Handlers;

internal class FoldingRangeHandler : IFoldingRangeHandler {
  public FoldingRangeRegistrationOptions GetRegistrationOptions() {
    return new FoldingRangeRegistrationOptions {
      DocumentSelector = DocumentSelector.ForLanguage("rad")
    };
  }


  public FoldingRangeRegistrationOptions GetRegistrationOptions(
    FoldingRangeCapability capability,
    ClientCapabilities clientCapabilities
  ) {
    return new FoldingRangeRegistrationOptions {
      DocumentSelector = DocumentSelector.ForLanguage("rad")
    };
  }


  public Task<Container<FoldingRange>?> Handle(
    FoldingRangeRequestParam request,
    CancellationToken cancellationToken
  ) {
    return Task.FromResult<Container<FoldingRange>?>(
        new Container<FoldingRange>(
            new FoldingRange {
              StartLine      = 10,
              EndLine        = 20,
              Kind           = FoldingRangeKind.Region,
              EndCharacter   = 0,
              StartCharacter = 0
            }
          )
      );
  }
}
