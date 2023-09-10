using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node;

public enum OperatorType {
  Star,
  ForwardSlash,
  Plus,
  Minus
}

/// <summary>
///   Represents an operator in the source code. For example, in the line <c> var x = 5 + 5; </c>,
///   <c> + </c> is an operator.
/// </summary>
/// <inheritdoc />
public class Operator : TokenNode {
  public OperatorType Type { get; internal set; }

  public Operator(ParserRuleContext context) : base(context) {}


  public Operator(ITerminalNode? tokenNode) :
    base(tokenNode) {}
}
