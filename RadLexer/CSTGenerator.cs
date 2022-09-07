using Antlr4.Runtime;

namespace RadLexer;

public class CSTGenerator {
  /// <summary>
  ///   Creates a lexer, tokenizes the input stream, creates a parser, adds an error
  ///   listener to the parser, parses the tokens, and returns the CST and any syntax errors
  /// </summary>
  /// <param name="sourceCodeString"> A string which represents the source code. </param>
  /// <returns>
  ///   A tuple containing a list of syntax errors and the CST.
  /// </returns>
  public (IEnumerable<SyntaxError>, Rad.StartRuleContext)
    GenerateCST(string sourceCodeString) {
    return GenerateCST(new AntlrInputStream(sourceCodeString));
  }


  /// <summary>
  ///   Creates a lexer, tokenizes the input stream, creates a parser, adds an error
  ///   listener to the parser, parses the tokens, and returns the CST and any syntax errors
  /// </summary>
  /// <param name="fileStream"> This is the file stream that contains the source code. </param>
  /// <returns>
  ///   A tuple containing a list of syntax errors and the CST.
  /// </returns>
  public (IEnumerable<SyntaxError>, Rad.StartRuleContext)
    GenerateCST(FileStream fileStream) {
    return GenerateCST(new AntlrInputStream(fileStream));
  }


  /// <summary>
  ///   Creates a lexer, tokenizes the input stream, creates a parser, adds an error
  ///   listener to the parser, parses the tokens, and returns the CST and any syntax errors
  /// </summary>
  /// <param name="inputStream"> This is the input stream that contains the source code. </param>
  /// <returns>
  ///   A tuple containing a list of syntax errors and the CST.
  /// </returns>
  public (IEnumerable<SyntaxError>, Rad.StartRuleContext)
    GenerateCST(AntlrInputStream inputStream) {
    // Create a lexer based on the input stream.
    var lexer = new RadLexer(inputStream);

    // Tokenize the file and get the token stream from the lexer.
    var tokenStream = new CommonTokenStream(lexer);

    // Create a token parser.
    var parser = new Rad(tokenStream);

    // Create a listener that handles syntax errors.
    var errorListener = new SyntaxErrorListener();

    // Add error listener for handling syntax errors.
    parser.AddErrorListener(errorListener);

    // Parse the tokens to generate the "Concrete Syntax Tree".
    var cst = parser.startRule();

    return (errorListener.Errors, cst);
  }
}
