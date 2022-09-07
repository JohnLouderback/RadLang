using System.Text.RegularExpressions;
using Antlr4.Runtime;

namespace RadParser.AST.Node;

public class NumericLiteral : Literal {
  public int Value => int.Parse(Regex.Replace(Text, "@\\D", ""));

  /// <inheritdoc />
  public override bool IsStaticConstant => true;

  public NumericLiteral(ParserRuleContext context) : base(context) {}
}
