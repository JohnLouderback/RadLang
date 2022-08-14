﻿/// This file is generated by `BaseASTVisitor.tt`. Please do not edit directly as it will be overwritten.

namespace RadParser.AST.Node;

public abstract class BaseASTVisitor<T> {
  public abstract T Visit(BinaryOperation node);

  public abstract T Visit(FunctionCallExpression node);

  public abstract T Visit(FunctionDeclaration node);

  public abstract T Visit(FunctionScopeStatement node);

  public abstract T Visit(LiteralExpression node);

  public abstract T Visit(Modifier node);

  public abstract T Visit(Module node);

  public abstract T Visit(NamedTypeParameter node);

  public abstract T Visit(NumericLiteral node);

  public abstract T Visit(OperationalKeyword node);

  public abstract T Visit(Operator node);

  public abstract T Visit(PositionalParameter node);

  public abstract T Visit(ReferenceExpression node);

  public abstract T Visit(Statement node);

  public abstract T Visit(TopLevel node);

  public abstract T Visit(TypeIdentifier node);

  public abstract T Visit(TypeReference node);

  public abstract T Visit(Value node);

  public abstract T Visit(Void node);


  public T Visit(INode node) {
    return Visit((dynamic)node);
  }
}
