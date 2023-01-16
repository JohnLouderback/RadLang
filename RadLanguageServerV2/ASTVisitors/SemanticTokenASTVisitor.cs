using RadLanguageServerV2.Constructs;
using RadParser;
using RadParser.AST.Node;
using RadParser.Utils;
using Void = RadParser.AST.Node.Void;

namespace RadLanguageServerV2.ASTVisitors;

public class SemanticTokenASTVisitor : BaseASTVisitor {
  private readonly List<(INode, SemanticTokenType, SemanticTokenModifier[])> tokens = new();
  public SemanticTokensDocument TokensDocument { get; }


  public SemanticTokenASTVisitor(SemanticTokensDocument document) {
    TokensDocument = document;
  }


  /// <summary>
  ///   Takes the list of tokens found during AST traversal and adds them to the list of semantic tokens
  ///   with the correct deltas of line numbers and column numbers.
  /// </summary>
  public void BuildTokens() {
    // Iterate over all tokens found during AST traversal and build the tokens with the correct deltas.
    foreach (var (node, semanticTokenType, modifiers) in tokens) {
      // Normalize the line number to be zero-based rather than 1 based.
      var line = node.Line - 1;

      // Build the semantic token array with correct deltas.
      TokensDocument.PushToken(
          line,
          node.Column,
          node.Width,
          semanticTokenType,
          modifiers
        );
    }
  }


  public override void Visit(BinaryOperation node) {
    Visit(node.LeftOperand);
    PushToken(node.Operator, SemanticTokenType.Operator);
    Visit(node.RightOperand);
  }


  public override void Visit(FunctionCallExpression node) {
    PushToken(node.Reference.Identifier, SemanticTokenType.Function);
    foreach (var arg in node.Arguments) {
      Visit(arg);
    }
  }


  public override void Visit(FunctionDeclaration node) {
    PushToken(node.Keyword, SemanticTokenType.Keyword);
    PushToken(node.Identifier, SemanticTokenType.Function, SemanticTokenModifier.Declaration);

    foreach (var param in node.Parameters) {
      Visit(param);
    }

    Visit(node.ReturnType);

    Visit(node.Body);
  }


  public override void Visit(LiteralExpression node) {
    Visit(node.Literal);
  }


  public override void Visit(Modifier node) {
    PushToken(node, SemanticTokenType.Modifier);
  }


  public override void Visit(NamedTypeParameter node) {
    PushToken(node.Identifier, SemanticTokenType.Parameter, SemanticTokenModifier.Declaration);

    Visit(node.TypeReference);
  }


  public override void Visit(NumericLiteral node) {
    PushToken(node, SemanticTokenType.Number);
  }


  public override void Visit(Operator node) {
    PushToken(node, SemanticTokenType.Operator);
  }


  public override void Visit(ReferenceExpression node) {
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
  }


  public override void Visit(Statement node) {
    if (node.LeadingKeyword is not null) {
      PushToken(node.LeadingKeyword, SemanticTokenType.Keyword);
    }

    if (node.Expression is {} expr) {
      Visit(expr);
    }
  }


  public override void Visit(TypeIdentifier node) {
    PushToken(node, SemanticTokenType.Keyword);
  }


  public override void Visit(TypeReference node) {
    PushToken(node, SemanticTokenType.Keyword);
  }


  public override void Visit(Void node) {
    PushToken(node, SemanticTokenType.Keyword);
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
