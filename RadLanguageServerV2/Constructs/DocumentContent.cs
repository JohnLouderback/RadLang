using RadLexer;
using RadParser;
using RadParser.AST.Node;

namespace RadLanguageServerV2.Constructs;

/// <summary>
///   The <c> DocumentContent </c> class represents the content of a document. It contains the raw text
///   of the document, the abstract syntax tree for the document, and any syntax errors found during
///   lexing and parsing.
/// </summary>
public class DocumentContent {
  /// <summary>
  ///   The raw text of the document.
  /// </summary>
  public string Text { get; private set; }

  /// <summary>
  ///   The array of lines in the document.
  /// </summary>
  public string[] Lines => Text.Split('\n');

  /// <summary>
  ///   The abstract syntax tree for the document.
  /// </summary>
  public INode? AST { get; private set; }

  /// <summary>
  ///   A list of syntax errors in the document, if any were found during lexing and parsing.
  /// </summary>
  public IEnumerable<SyntaxError> SyntaxErrors { get; private set; } = new List<SyntaxError>();


  /// <inheritdoc cref="DocumentContent" />
  /// <param name="text"> The text to set the document content to. </param>
  public DocumentContent(string text) {
    Text = text;
    AST  = GenerateAST();
  }


  /// <summary>
  ///   Update the text to the provided string. This replaces the entire text content.
  /// </summary>
  /// <param name="text"> The text to set the document content to. </param>
  /// <returns> `this` for chaining. </returns>
  public DocumentContent Update(string text) {
    Text = text;
    AST  = GenerateAST();
    return this;
  }


  private INode GenerateAST() {
    // Get the concrete syntax tree for the document.
    var (errors, cst) = new CSTGenerator().GenerateCST(Text);

    // Store any syntax errors found during lexing and parsing for the CST.
    SyntaxErrors = errors;

    // Return the AST.
    return new ASTGenerator().GenerateASTFromCST(cst);
  }
}
