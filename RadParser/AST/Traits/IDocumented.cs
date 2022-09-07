using RadParser.Constructs;

namespace RadParser.AST.Traits;

/// <summary>
/// The trait of a node that has documentation defined.
/// </summary>
public interface IDocumented {
  Documentation Documentation { get; }
}
