using RadParser.Utils;

namespace RadParser.AST.Node;

/// <summary>
///   <para>
///     Represents a reference to a declaration in the source code. For example, in the code:
///     <code>
///       var x = 5;
///       x += 5;
///     </code>
///     <c> x </c> in the second line is a reference to the declaration of <c> x </c> in the first
///     line.
///   </para>
///   <para>
///     To find the declaration that this <see cref="Reference" /> refers to, use the
///     <see cref="ASTNodeExtensions.GetDeclaration{T}(IReference{T})" /> extension method.
///   </para>
/// </summary>
public interface IReference<T> : INode where T : Identifier {
  /// <summary>
  ///   The <see cref="Identifier" /> that this <see cref="IReference{T}" /> is identified by.
  /// </summary>
  public T Identifier { get; init; }
}
