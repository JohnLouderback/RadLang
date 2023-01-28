using Antlr4.Runtime;

namespace RadParser.AST.Node;

public class FunctionDeclaration : Declaration {
  public List<NamedTypeParameter> Parameters { get; internal set; }

  /// <summary>
  ///   References the colon token that denotes a return type annotation given in the function declaration.
  /// </summary>
  public Token ReturnTypeColon { get; internal set; }

  public TypeReference ReturnType { get; internal set; }
  public FunctionScope Body { get; internal set; }

  /// <inheritdoc />
  public override bool IsStaticConstant => true;

  public FunctionDeclaration(ParserRuleContext context) : base(context) {}


  /// <summary>
  ///   Returns true if the function is pure. That is, the function does not reference any declarations
  ///   outside of its own scope and does not have any side-effects.
  /// </summary>
  /// <returns> A boolean value indicating whether the function is pure. </returns>
  public bool IsFunctionPure() {
    var references = Body.GetAllReferencesInScope();

    // A function is pure if there are no references at all, of course, for whatever reason that might be.
    if (references.Count == 0) return true;

    // If all references belong to the scope of this function, the function is pure.
    if (references.TrueForAll(reference => reference.GetParentScope() == Body)) {
      return true;
    }

    return false;
  }
}
