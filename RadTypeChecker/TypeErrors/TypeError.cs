﻿using RadDiagnostics;
using RadParser.AST.Node;
using RadUtils;
using RadUtils.Constructs;

namespace RadTypeChecker.TypeErrors;

/// <summary>
///   Denotes a type error found during type checking of the AST.
/// </summary>
public abstract class TypeError<T> : ITypeError, ITypeErrorForNode<T> where T : INode {
  /// <inheritdoc />
  public virtual T ForNode { get; }

  /// <summary>
  ///   The message explaining the cause of the error.
  /// </summary>
  public virtual string Message { get; }

  /// <inheritdoc />
  public uint ErrorCode =>
    // Gets the error code from the `TypeErrors` enum based on the name of the current subclass.
    EnumUtils.GetValueOf<TypeErrors, uint>(GetType().Name);

  /// <inheritdoc />
  public virtual string ErrorCodeString => $"RTE-{ErrorCode.ToString().PadLeft(4, '0')}";

  /// <inheritdoc />
  public virtual DiagnosticSeverity Severity => DiagnosticSeverity.Error;

  public SourceCodeLocation Location => ForNode.Location;


  /// <param name="node"> The node at which the error occurred. </param>
  /// <param name="message"> The message explaining the cause of the error. </param>
  public TypeError(T node, string message) {
    ForNode = node;
    Message = $"{GetType().Name.PascalCaseToTitleCase()}: {message}";
  }
}
