using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using RadParser.Constructs;

namespace RadParser.AST.Node;

public enum DeclaratorKeywordType {
  Function
}

public class DeclaratorKeyword : Keyword, IDocumented {
  public const string FunctionKeywordDocumentation = """
  The `fn` keyword denotes a function declaration. It should be followed by an identifier, or name, for the function.
  """;
  public DeclaratorKeywordType Type { get; internal set; }
  public DeclaratorKeyword(ParserRuleContext context) : base(context) {}

  public DeclaratorKeyword(ITerminalNode token) : base(token) {}

  public Documentation Documentation => Type switch {
    DeclaratorKeywordType.Function => new Documentation { Markdown = FunctionKeywordDocumentation }
  };
}
