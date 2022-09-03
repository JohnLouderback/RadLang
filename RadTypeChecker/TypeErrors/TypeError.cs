using RadDiagnostics;
using RadParser.AST.Node;
using RadUtils;

namespace RadTypeChecker.TypeErrors;

/// <summary>
///   Denotes a type error found during type checking of the AST.
/// </summary>
public abstract class TypeError<T> : ITypeError, ITypeErrorForNode<T> where T : INode {
  /// <inheritdoc />
  public virtual T ForNode { get; }

  /// <inheritdoc />
  public virtual string Message { get; }

  /// <inheritdoc />
  public abstract uint ErrorCode { get; }

  /// <inheritdoc />
  public virtual string ErrorCodeString => $"RTE-{ErrorCode.ToString().PadLeft(4, '0')}";

  /// <inheritdoc />
  public virtual DiagnosticSeverity Severity => DiagnosticSeverity.Error;


  /// <param name="node"> The node at which the error occurred. </param>
  /// <param name="message"> The message explaining the cause of the error. </param>
  public TypeError(T node, string message) {
    ForNode = node;
    Message = $"{GetType().Name.PascalCaseToTitleCase()}: {message}";
  }
}
