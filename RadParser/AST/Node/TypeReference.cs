using Antlr4.Runtime.Tree;

namespace RadParser.AST.Node;

/// <summary>
///   <para>
///     Represents a reference to a type declaration in the source code. For example, in the code:
///     <code>
///       var x: int = 5;
///     </code>
///     <c> int </c> is a reference to the declaration of the type <c> int </c>.
///   </para>
///   <para>
///     To find the declaration that this <see cref="T:RadParser.AST.Node.Reference" /> refers to, use the
///     <see cref="M:RadParser.Utils.ASTNodeExtensions.GetDeclaration``1(RadParser.AST.Node.IReference{``0})" /> extension
///     method.
///   </para>
/// </summary>
/// <inheritdoc cref="IReference{T}" />
public class TypeReference : TokenNode, IReference<TypeIdentifier> {
  public required TypeIdentifier Identifier { get; init; }
  public TypeReference(ITerminalNode? tokenNode) : base(tokenNode) {}
}
