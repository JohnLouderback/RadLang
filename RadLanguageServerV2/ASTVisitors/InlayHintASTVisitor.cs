using Microsoft.VisualStudio.LanguageServer.Protocol;
using RadLanguageServerV2.LanguageServerEx.Models.InlayHint;
using RadParser;
using RadParser.AST.Node;
using RadParser.Utils;

namespace RadLanguageServerV2.ASTVisitors;

public class InlayHintASTVisitor : BaseASTVisitor {
  public List<InlayHint> InlayHints { get; } = new();


  public override void Visit(FunctionCallExpression node) {
    if (node.Reference?.GetDeclaration() is FunctionDeclaration funcDecl) {
      var argIndex = 0;
      // Add name annotations to arguments.
      foreach (var argument in node.Arguments) {
        InlayHints.Add(
            new InlayHint {
              Position = new Position {
                Character = argument.Column,
                Line      = argument.Line - 1 // Subtract one to convert line to 0-based.
              },
              Kind  = InlayHintKind.Parameter,
              Label = $"{funcDecl.Parameters[argIndex].Identifier.Name}: "
            }
          );
        argIndex++;
      }

      // Add type annotation to return type.
      InlayHints.Add(
          new InlayHint {
            Position = new Position {
              Character = node.Column + node.Width,
              Line      = node.Line - 1 // Subtract one to convert line to 0-based.
            },
            Kind  = InlayHintKind.Type,
            Label = $": {funcDecl.ReturnType.Identifier.Name}"
          }
        );
    }
  }
}
