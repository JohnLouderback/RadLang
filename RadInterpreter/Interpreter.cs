using Antlr4.Runtime;
using RadCompiler;
using RadParser;
using RadParser.AST.Node;
using RadParser.Utils;

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

    // Parse the tokens to generate the "Concrete Syntax Tree".
    var cst = parser.startRule();
    var ast = (new ASTGenerator()).GenerateASTFromCST(cst);
    new Compiler().Compile(ast);
    
  }
}
