using Antlr4.Runtime;
using OneOf;

namespace RadParser.AST.Node; 

[GenerateOneOf]
public partial class Value : OneOfBase<Literal, Expression, Identifier> { }
