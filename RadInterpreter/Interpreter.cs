using Antlr4.Runtime;
using RadParser;

namespace RadInterpreter;

public class Interpreter {
  public void InterpretFile(string filePath) {
    // Get the input stream from the file path provided.
    var inputStream = new AntlrInputStream(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
    
    // Create a lexer based on the input stream.
    var lexer = new RadLexer.RadLexer(inputStream);
    
    // Tokenize the file and get the token stream from the lexer.
    var tokenStream = new CommonTokenStream(lexer);
    
    // Create a token parser.
    var parser = new RadLexer.Rad(tokenStream);

    try {
      // Parse the tokens to generate the "Concrete Syntax Tree".
      var cst = parser.startRule();
      var ast = (new ASTGenerator()).GenerateASTFromCST(cst);
      Console.WriteLine(ast);
    }
    catch (Exception e) {
      Console.WriteLine(e);
    }
  }
}
