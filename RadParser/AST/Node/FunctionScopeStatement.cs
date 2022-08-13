using Antlr4.Runtime;
using OneOf;

namespace RadParser.AST.Node; 

[GenerateOneOf]
public partial class FunctionScopeStatement : OneOfBase<Statement, Declaration> { }
