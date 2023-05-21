using RadParser;
using RadParser.AST.Node;

namespace RadProject;

public class Project {
  public Module EntryPoint { get; private set; } = null!;
  private static ASTGenerator ASTGenerator { get; } = new();


  public static Project FromEntryPoint(string entryPoint) {
    var entryPointModule = ASTGenerator.ParseFile(entryPoint);
    return new Project {
      EntryPoint = entryPointModule
    };
  }
}
