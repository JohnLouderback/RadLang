using Antlr4.Runtime;

namespace RadParser.AST.Node;

public class NamedTypeParameter : Declaration {
  public TypeReference TypeReference { get; internal set; }

  /// <summary>
  ///   References the colon token that denotes a type annotation.
  /// </summary>
  public Token Colon { get; internal set; }

  /// <inheritdoc />
  public override bool IsStaticConstant =>
    false; // Parameters are variable by their very nature, so they cannot be considered static constants.

  public NamedTypeParameter(ParserRuleContext context) : base(context) {}
}
