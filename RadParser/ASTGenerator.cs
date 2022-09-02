using RadLexer;
using RadParser.AST.Node;
using RadParser.Utils;

namespace RadParser;

public class ASTGenerator {
  public Module GenerateASTFromCST(Rad.StartRuleContext cst) {
    return ASTUtils.Ophanarium(new ASTBuilderVisitor().VisitStartRule(cst));
  }
}
