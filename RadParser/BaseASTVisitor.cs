using RadParser.AST.Node;
using Void = RadParser.AST.Node.Void;

namespace RadParser;

public abstract class BaseASTVisitor : IBaseASTVisitor {
  public virtual void Visit(OperationalKeyword node) {}


  public virtual void Visit(Statement node) {
    if (node.LeadingKeyword is {} keyword) {
      Visit(keyword);
    }

    if (node.Expression is {} expr) {
      Visit(expr);
    }
  }


  public virtual void Visit(TopLevel node) {
    foreach (var statement in node.AllStatements) {
      Visit(statement);
    }
  }


  public virtual void Visit(Expression node) {
    switch (node) {
      case FunctionCallExpression funcExpr:
        Visit(funcExpr);
        break;
      case BinaryOperation binOp:
        Visit(binOp);
        break;
      case ReferenceExpression refExpr:
        Visit(refExpr);
        break;
    }
  }


  public virtual void Visit(BinaryOperation node) {
    Visit(node.LeftOperand);
    Visit(node.Operator);
    Visit(node.RightOperand);
  }


  public virtual void Visit(DeclaratorKeyword node) {}


  public virtual void Visit(FunctionCallExpression node) {
    Visit(node.Reference);
    foreach (var arg in node.Arguments) {
      Visit(arg);
    }
  }


  public virtual void Visit(FunctionDeclaration node) {
    Visit(node.Keyword);
    Visit(node.Identifier);
    foreach (var param in node.Parameters) {
      Visit(param);
    }

    Visit(node.ReturnType);
    Visit(node.Body);
  }


  public virtual void Visit(FunctionScope node) {
    foreach (var statement in node.AllStatements) {
      Visit(statement);
    }
  }


  public virtual void Visit(FunctionScopeStatement node) {
    switch (node) {
      case { Value: Statement stmnt }:
        Visit(stmnt);
        break;
      case { Value: Declaration declaration }:
        Visit(declaration);
        break;
    }
  }


  public virtual void Visit(LiteralExpression node) {
    Visit(node.Literal);
  }


  public virtual void Visit(Modifier node) {}


  public virtual void Visit(Module node) {
    Visit(node.TopLevel);
  }


  public virtual void Visit(NamedTypeParameter node) {
    Visit(node.Identifier);
    Visit(node.TypeReference);
  }


  public virtual void Visit(NumericLiteral node) {}

  public virtual void Visit(Operator node) {}


  public virtual void Visit(PositionalParameter node) {
    Visit(node.Value);
  }


  public virtual void Visit(Reference node) {
    Visit(node.Identifier);
  }


  public virtual void Visit(ReferenceExpression node) {
    Visit(node.Reference);
  }


  public virtual void Visit(TypeIdentifier node) {}


  public virtual void Visit(TypeReference node) {
    Visit(node.Identifier);
  }


  public virtual void Visit(Value node) {
    Visit((dynamic)node.Value);
  }


  public virtual void Visit(Void node) {}


  public virtual void Visit(Declaration node) {
    switch (node) {
      case FunctionDeclaration funcDecl:
        Visit(funcDecl);
        break;
    }
  }


  public virtual void Visit(Literal node) {
    switch (node) {
      case NumericLiteral numLiteral:
        Visit(numLiteral);
        break;
    }
  }


  public virtual void Visit(IReference<Identifier> node) {
    switch (node) {
      case TypeReference typeRef:
        Visit(typeRef);
        break;
      case Reference reference:
        Visit(reference);
        break;
    }
  }


  public virtual void Visit(Identifier node) {}


  public void Visit(INode node) {
    Visit((dynamic)node);
  }
}
