using Antlr4.Runtime;
using RadParser.Utils;

namespace RadParser.AST.Node;

public class FunctionCallExpression : Expression {
  public Reference Reference { get; internal set; }
  public List<PositionalParameter> Arguments { get; internal set; }

  /// <inheritdoc />
  public override bool IsStaticConstant =>
    // If all arguments are static constants...
    Arguments.TrueForAll(arg => arg.IsStaticConstant) &&
    // ...and the function is pure (it has no side-effects)...
    Reference.GetDeclaration() is FunctionDeclaration funcDecl &&
    funcDecl.IsFunctionPure(); // ...then this function call is considered to be a static constant itself.

  public FunctionCallExpression(ParserRuleContext context) : base(context) {}
}
