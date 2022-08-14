using LLVMSharp.Interop;
using RadParser;
using RadParser.AST.Node;
using Void = RadParser.AST.Node.Void;

namespace RadCompiler; 

public class CodeGenASTVisitor : BaseASTVisitor<LLVMBuilderRef> {
  
  private readonly LLVMModuleRef module;

  private readonly LLVMBuilderRef builder;

  private readonly Dictionary<string, LLVMValueRef> namedValues = new Dictionary<string, LLVMValueRef>();
  
  public override LLVMBuilderRef Visit(BinaryOperation node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(FunctionCallExpression node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(FunctionDeclaration node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(FunctionScopeStatement node) {
    throw new NotImplementedException();
  }

  public override LLVMBuilderRef Visit(LiteralExpression node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(Modifier node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(Module node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(NamedTypeParameter node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(NumericLiteral node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(OperationalKeyword node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(Operator node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(PositionalParameter node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(ReferenceExpression node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(Statement node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(TopLevel node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(TypeIdentifier node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(TypeReference node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(Value node) {
    throw new NotImplementedException();
  }


  public override LLVMBuilderRef Visit(Void node) {
    throw new NotImplementedException();
  }
}
