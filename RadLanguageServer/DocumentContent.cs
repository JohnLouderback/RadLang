using Antlr4.Runtime;
using RadLexer;
using RadParser;
using RadParser.AST.Node;

namespace RadLanguageServer;

public class DocumentContent {
  public string Text { get; private set; }
  public string[] Lines => Text.Split('\n');
  public INode AST { get; private set; }


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
    // Get the input stream from the file content provided.
    var inputStream = new AntlrInputStream(
        Text
      );

    // Create a lexer based on the input stream.
    var lexer = new RadLexer.RadLexer(inputStream);

    // Tokenize the file and get the token stream from the lexer.
    var tokenStream = new CommonTokenStream(lexer);

    // Create a token parser.
    var parser = new Rad(tokenStream);

    // Parse the tokens to generate the "Concrete Syntax Tree".
    var cst = parser.startRule();

    // Return the AST.
    return new ASTGenerator().GenerateASTFromCST(cst);
  }
}
