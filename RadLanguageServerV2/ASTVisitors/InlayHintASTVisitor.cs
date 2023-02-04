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
        // If the number of arguments is greater than the number of defined parameters, break of of the loop.
        // This can happen when a syntax error affects a function signature.
        if (funcDecl.Parameters.Count - 1 <= argIndex) break;

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

      // Add type annotation to return type. The return type maybe be null if there is a syntax error
      // or if the function signature is still being written.
      if (funcDecl.ReturnType?.Identifier is not null) {
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
}
