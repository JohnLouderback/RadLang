using Antlr4.Runtime;
using RadCompiler;
using RadLexer;
using RadParser;

namespace RadInterpreter;

public class Interpreter {
  /// <summary>
  ///   The `Status` event is used to send status updates to subscribers.
  /// </summary>
  public event Compiler.CompilerEventHandler Status;


  public void InterpretFile(string filePath) {
    // Get the input stream from the file path provided.
    var inputStream = new AntlrInputStream(
        File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
      );

    // Create a lexer based on the input stream.
    var lexer = new RadLexer.RadLexer(inputStream);

    // Tokenize the file and get the token stream from the lexer.
    var tokenStream = new CommonTokenStream(lexer);

    // Create a token parser.
    var parser = new Rad(tokenStream);

    // Parse the tokens to generate the "Concrete Syntax Tree".
    var cst      = parser.startRule();
    var ast      = new ASTGenerator().GenerateASTFromCST(cst);
    var compiler = new Compiler();
    compiler.Status += (sender, args) => { Status?.Invoke(sender, args); };
    compiler.Compile(ast);
  }
}
