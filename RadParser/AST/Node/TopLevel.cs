using Antlr4.Runtime;

namespace RadParser.AST.Node; 

public class TopLevel : Scope<FunctionScopeStatement> {
  


  public TopLevel(ParserRuleContext context) : base(context) {
  }
}
