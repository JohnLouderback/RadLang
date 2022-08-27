using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using RadParser;
using RadParser.AST.Node;
using RadParser.Utils;
using Void = RadParser.AST.Node.Void;

namespace RadLanguageServer;

public class SemanticTokenASTVisitor : BaseASTVisitor<object?> {
  private readonly List<(INode, SemanticTokenType, SemanticTokenModifier[])> tokens = new();
  public SemanticTokensBuilder TokensBuilder { get; }


  public SemanticTokenASTVisitor(SemanticTokensBuilder builder) {
    TokensBuilder = builder;
  }


  /// <summary>
  ///   Takes the list of tokens found during AST traversal and adds them to the list of semantic tokens
  ///   with the correct deltas of line numbers and column numbers.
  /// </summary>
  public void BuildTokens() {
    // Set the initial values for the last line - used to calculate the deltas. The builder seems to
    // calculate the delta for columns, but not lines, interestingly.
    var lastLine = 0;

    // Iterate over all tokens found during AST traversal and build the tokens with the correct deltas.
    foreach (var (node, semanticTokenType, modifiers) in tokens) {
      // Normalize the line number to be zero-based rather than 1 based.
      var line = node.Line - 1;

      // Build the semantic token array with correct deltas.
      TokensBuilder.Push(
          line,
          node.Column,
          node.Width,
          semanticTokenType,
          modifiers
        );

      // Update the last line and column.
      lastLine = line;
    }
  }


  public override object? Visit(BinaryOperation node) {
    Visit(node.LeftOperand);
    PushToken(node.Operator, SemanticTokenType.Operator);
    Visit(node.RightOperand);

    return null;
  }


  public override object? Visit(DeclaratorKeyword node) {
    throw new NotImplementedException();
  }


  public override object? Visit(FunctionCallExpression node) {
    PushToken(node.Reference.Identifier, SemanticTokenType.Function);
    foreach (var arg in node.Arguments) {
      Visit(arg);
    }

    return null;
  }


  public override object? Visit(FunctionDeclaration node) {
    PushToken(node.Keyword, SemanticTokenType.Keyword);
    PushToken(node.Identifier, SemanticTokenType.Function, SemanticTokenModifier.Declaration);

    foreach (var param in node.Parameters) {
      Visit(param);
    }

    Visit(node.ReturnType);

    Visit(node.Body);

    return null;
  }


  public override object? Visit(FunctionScope node) {
    foreach (var statement in node.AllStatements) {
      Visit(statement);
    }

    return null;
  }


  public override object? Visit(FunctionScopeStatement node) {
    switch (node) {
      case { Value: Statement stmnt }:
        Visit(stmnt);
        break;
      case { Value: Declaration declaration }:
        Visit(declaration);
        break;
    }

    return null;
  }


  public override object? Visit(LiteralExpression node) {
    Visit(node.Literal);

    return null;
  }


  public override object? Visit(Modifier node) {
    PushToken(node, SemanticTokenType.Modifier);

    return null;
  }


  public override object? Visit(Module node) {
    Visit(node.TopLevel);

    return null;
  }


  public override object? Visit(NamedTypeParameter node) {
    PushToken(node.Identifier, SemanticTokenType.Parameter, SemanticTokenModifier.Declaration);

    Visit(node.TypeReference);

    return null;
  }


  public override object? Visit(NumericLiteral node) {
    PushToken(node, SemanticTokenType.Number);

    return null;
  }


  public override object? Visit(OperationalKeyword node) {
    throw new NotImplementedException();
  }


  public override object? Visit(Operator node) {
    PushToken(node, SemanticTokenType.Operator);

    return null;
  }


  public override object? Visit(PositionalParameter node) {
    Visit(node.Value);

    return null;
  }


  public override object? Visit(ReferenceExpression node) {
    var decl = node.Reference.GetDeclaration();
    // Set the token type based on the type of the declaration it references.
    switch (decl) {
      case FunctionDeclaration:
        PushToken(node, SemanticTokenType.Function);
        break;
      case NamedTypeParameter:
        PushToken(node, SemanticTokenType.Parameter);
        break;
    }

    return null;
  }


  public override object? Visit(Statement node) {
    if (node.LeadingKeyword is not null) {
      PushToken(node.LeadingKeyword, SemanticTokenType.Keyword);
    }

    if (node.Expression is {} expr) {
      Visit(expr);
    }

    return null;
  }


  public override object? Visit(TopLevel node) {
    foreach (var statement in node.AllStatements) {
      Visit(statement);
    }

    return null;
  }


  public override object? Visit(TypeIdentifier node) {
    PushToken(node, SemanticTokenType.Keyword);

    return null;
  }


  public override object? Visit(TypeReference node) {
    PushToken(node, SemanticTokenType.Keyword);

    return null;
  }


  public override object? Visit(Value node) {
    switch (node.Value) {
      case Literal literal:
        Visit(literal);
        break;
      case Expression expr:
        Visit(expr);
        break;
    }

    return null;
  }


  public override object? Visit(Void node) {
    PushToken(node, SemanticTokenType.Keyword);

    return null;
  }


  public override object? Visit(Expression node) {
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

    return null;
  }


  public override object? Visit(Declaration node) {
    if (node is FunctionDeclaration funcExpr) {
      Visit(funcExpr);
    }

    return null;
  }


  public override object? Visit(Literal node) {
    switch (node) {
      case NumericLiteral numLiteral:
        Visit(numLiteral);
        break;
    }

    return null;
  }


  private void PushToken(
    INode node,
    SemanticTokenType type,
    params SemanticTokenModifier[]
      modifiers
  ) {
    tokens.Add(
        (
          node,
          type,
          modifiers
        )
      );
  }
}
