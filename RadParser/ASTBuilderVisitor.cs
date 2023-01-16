using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using OneOf;
using RadLexer;
using RadParser.AST.Node;
using RadParser.Utils;
using Void = RadParser.AST.Node.Void;

namespace RadParser;

public class ASTBuilderVisitor : RadBaseVisitor<INode> {
  public override INode Visit(IParseTree tree) {
    return base.Visit(tree);
  }


  public override INode VisitBinaryOperator(Rad.BinaryOperatorContext context) {
    return base.VisitBinaryOperator(context);
  }


  public override Declaration VisitDeclaration(Rad.DeclarationContext context) {
    if (context.functionDeclaration() is not null) {
      return VisitFunctionDeclaration(context.functionDeclaration());
    }

    throw new Exception($"\"{context.GetFullText()}\" is an unknown declaration type.");
  }


  public override Statement VisitDefiniteStatement(Rad.DefiniteStatementContext context) {
    return new Statement(context) {
      LeadingKeyword = context.statementKeyword() is not null
                         ? VisitStatementKeyword(context.statementKeyword())
                         : null,
      Expression = VisitExpression(context.expression())
    };
  }


  public override Expression VisitExpression(Rad.ExpressionContext context) {
    if (context?.functionCall() is not null) {
      return VisitFunctionCall(context.functionCall());
    }

    if (context?.literal() is not null) {
      return new LiteralExpression(context.literal()) {
        Literal = VisitLiteral(context.literal())
      };
    }

    if (context?.ID() is not null) {
      return new ReferenceExpression(context) {
        Reference = new Reference(context.ID()) {
          Identifier = new Identifier(context.ID())
        }
      };
    }

    // If this expression is an operation.
    if (context?.op is not null) {
      (ITerminalNode? operatorNode, OperatorType operatorType) = (default, default);

      if (context.op == context.STAR()?.Payload) {
        (operatorNode, operatorType) = (context.STAR(), OperatorType.Star);
      }
      else if (context.op == context.FSLASH()?.Payload) {
        (operatorNode, operatorType) = (context.FSLASH(), OperatorType.ForwardSlash);
      }
      else if (context.op == context.PLUS()?.Payload) {
        (operatorNode, operatorType) = (context.PLUS(), OperatorType.Plus);
      }
      else if (context.op == context.MINUS()?.Payload) {
        (operatorNode, operatorType) = (context.MINUS(), OperatorType.Minus);
      }

      // If there is an operator and two operands, it's a binary expression.
      if (context.expression(0) is not null &&
          context.expression(1) is not null) {
        return new BinaryOperation(context) {
          LeftOperand  = VisitExpression(context.expression(0)),
          RightOperand = VisitExpression(context.expression(1)),
          Operator = new Operator(operatorNode) {
            Type = operatorType
          }
        };
      }
    }

    if (context?.ID() is not null) {
      return new ReferenceExpression(context) {
        Reference = new Reference(context.ID()) {
          Identifier = new Identifier(context.ID())
        }
      };
    }

    throw new Exception($"\"{context.GetFullText()}\" is not a known expression type.");
  }


  public override FunctionScope VisitFunctionBody(Rad.FunctionBodyContext context) {
    return new FunctionScope(context) {
      Children = VisitStatementGroup(context.statementGroup()).Children
    };
  }


  public override FunctionCallExpression VisitFunctionCall(Rad.FunctionCallContext context) {
    return new FunctionCallExpression(context) {
      Reference = new Reference(context.ID()) {
        Identifier = new Identifier(context.ID())
      },
      Arguments = VisitOrderedTuple(context.orderedTuple())
    };
  }


  public override FunctionDeclaration VisitFunctionDeclaration(
    Rad.FunctionDeclarationContext context
  ) {
    return new FunctionDeclaration(context) {
      Keyword = new DeclaratorKeyword(context.FN()) {
        Type = DeclaratorKeywordType.Function
      },
      Identifier = new Identifier(context.ID()),
      Parameters = VisitNamedTypeTuple(context.namedTypeTuple()),
      ReturnType = VisitReturnTypeSpecifier(context.returnTypeSpecifier()),
      Body       = VisitFunctionBody(context.functionBody())
    };
  }


  public override Literal VisitLiteral(Rad.LiteralContext context) {
    if (context.numericLiteral() is not null) {
      return VisitNumericLiteral(context.numericLiteral());
    }

    throw new Exception($"\"{context.GetFullText()}\" is not a known literal value.");
  }


  public override NamedTypeParameter VisitNamedParameter(Rad.NamedParameterContext context) {
    return new NamedTypeParameter(context) {
      Identifier    = new Identifier(context.ID()),
      TypeReference = VisitTypeSpecifier(context.typeSpecifier())
    };
  }


  public override NodeCollection<NamedTypeParameter> VisitNamedParameters(
    Rad.NamedParametersContext context
  ) {
    return new NodeCollection<NamedTypeParameter>(context) {
      Children = context.namedParameter().Select(param => VisitNamedParameter(param)).ToList()
    };
  }


  public override NodeCollection<NamedTypeParameter> VisitNamedTypeTuple(
    Rad.NamedTypeTupleContext context
  ) {
    return VisitNamedParameters(context.namedParameters());
  }


  public override TypeReference VisitNumberType(Rad.NumberTypeContext context) {
    ITerminalNode tokenNode;
    if (context.keyword == context.INT_KEYWORD()?.Payload) {
      tokenNode = context.INT_KEYWORD();
    }
    else if (context.keyword == context.FLOAT_KEYWORD()?.Payload) {
      tokenNode = context.FLOAT_KEYWORD();
    }
    else {
      throw new Exception($"Keyword \"{context.keyword.Text}\" is not a known numeric type.");
    }

    var typeReference = new TypeReference(tokenNode) {
      Identifier = new TypeIdentifier(tokenNode)
    };
    if (context.UNSIGNED() is not null) {
      typeReference.Identifier.Modifiers.Add(
          new Modifier(context.UNSIGNED()) {
            Type = ModifierType.Unsigned
          }
        );
    }

    return typeReference;
  }


  public override NumericLiteral VisitNumericLiteral(Rad.NumericLiteralContext context) {
    return new NumericLiteral(context);
  }


  public override PositionalParameter VisitOrderedParameter(Rad.OrderedParameterContext context) {
    Value value;

    if (context.ID() is not null) {
      value = new Value(new Identifier(context.ID()));
    }
    else if (context.literal() is not null) {
      value = new Value(VisitLiteral(context.literal()));
    }
    else {
      throw new Exception($"Parameter \"{context.GetFullText()}\" of tuple is not a known value.");
    }

    return new PositionalParameter(context) {
      Value = value
    };
  }


  public override NodeCollection<PositionalParameter> VisitOrderedParameters(
    Rad.OrderedParametersContext context
  ) {
    return new NodeCollection<PositionalParameter>(context) {
      Children = context.orderedParameter().Select(param => VisitOrderedParameter(param)).ToList()
    };
  }


  public override NodeCollection<PositionalParameter> VisitOrderedTuple(
    Rad.OrderedTupleContext context
  ) {
    return VisitOrderedParameters(context.orderedParameters());
  }


  public override TypeReference VisitReturnTypeSpecifier(Rad.ReturnTypeSpecifierContext context) {
    // ReSharper disable once ConvertIfStatementToReturnStatement
    if (context.voidSpecifier() is not null) return new Void(context.voidSpecifier().VOID());

    return VisitTypeSpecifier(context.typeSpecifier());
  }


  public override Module VisitStartRule(Rad.StartRuleContext context) {
    return new Module(context) {
      TopLevel = VisitTopLevel(context.topLevel())
    };
  }


  public override NodeCollection<FunctionScopeStatement> VisitStatementGroup(
    Rad.StatementGroupContext context
  ) {
    var children = context.GetRuleContexts<ParserRuleContext>();
    return new NodeCollection<FunctionScopeStatement>(context) {
      Children = children.Select(
            child => {
              OneOf<Statement, Declaration> node = child switch {
                Rad.DefiniteStatementContext statementCtx =>
                  VisitDefiniteStatement(statementCtx),
                Rad.DeclarationContext declCtx => VisitDeclaration(
                    declCtx
                  ),
                _ => throw new Exception(
                         $"Top-level statement was not of a known context type. Got: {
                           child.GetType().Name
                         }"
                       )
              };

              return new FunctionScopeStatement(node);
            }
          )
        .ToList()
    };
  }


  public override OperationalKeyword VisitStatementKeyword(Rad.StatementKeywordContext context) {
    return new OperationalKeyword(context) {
      Type = context.GetFullText() switch {
        "return" => OperationalKeywordType.Return,
        "out" => OperationalKeywordType.Out,
        _ => throw new Exception($"Keyword \"{context.GetFullText()}\" is not a known keyword.")
      }
    };
  }


  public override INode VisitStringLiteral(Rad.StringLiteralContext context) {
    return base.VisitStringLiteral(context);
  }


  public override TopLevel VisitTopLevel(Rad.TopLevelContext context) {
    var children = context.GetRuleContexts<ParserRuleContext>();
    var statements = children.Select(
        child => {
          OneOf<Statement, Declaration> node = child switch {
            Rad.DefiniteStatementContext statementCtx => VisitDefiniteStatement(statementCtx),
            Rad.DeclarationContext declCtx            => VisitDeclaration(declCtx),
            _ => throw new Exception(
                     $"Top-level statement was not of a known context type. Got: {
                       child.GetType().Name
                     }"
                   )
          };

          return new FunctionScopeStatement(node);
        }
      );

    return new TopLevel(context) {
      AllStatements = statements.ToList()
    };
  }


  public override TypeReference VisitTypeSpecifier(Rad.TypeSpecifierContext context) {
    if (context.numberType() is not null) return VisitNumberType(context.numberType());
    return new TypeReference(context.ID());
  }


  public override INode VisitVoidSpecifier(Rad.VoidSpecifierContext context) {
    return base.VisitVoidSpecifier(context);
  }
}
