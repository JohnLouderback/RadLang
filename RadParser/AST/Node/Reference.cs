using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using RadParser.AST.Traits;
using RadParser.Utils;
using RadParser.Utils.Attributes;

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
///
///   <para>
///     To find the declaration that this <see cref="Reference" /> refers to, use the
///     <see cref="ASTNodeExtensions.GetDeclaration{T}(IReference{T})" /> extension method.
///   </para>
/// </summary>
public class Reference : TokenNode, IReference<Identifier>, IPossibleConstant {
  /// <inheritdoc />
  public virtual required Identifier Identifier { get; init; }

  public Reference(ITerminalNode tokenNode) : base(tokenNode) {}


  /// <inheritdoc />
  public bool IsStaticConstant => (this as IReference<Identifier>).GetDeclaration()?.IsStaticConstant ?? false;
}
