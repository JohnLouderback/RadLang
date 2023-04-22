using Antlr4.Runtime;
using RadCompiler;
using RadLexer;
using RadParser;

namespace RadInterpreter;

/// <summary>
///   The <c> Interpreter </c> class is responsible for interpreting the AST. It may not strictly be
///   an interpreter per-se, as it does not interpret the AST directly. Instead, it generates LLVM IR
///   code from the AST and then uses the LLVM JIT to run the program. This can be used to "run" a
///   file as a "script" without having to compile it to an executable first. It also allows for
///   more dynamic and incremental development, as the program can be run at any time without having
///   to recompile it.
/// </summary>
public class Interpreter {
  /// <summary>
  ///   The `Status` event is used to send status updates to subscribers.
  /// </summary>
  public event Compiler.CompilerEventHandler Status;


  /// <summary>
  ///   Given an input file path, this method will parse and execute the code in the file.
  /// </summary>
  /// <param name="filePath"> The path to the file to interpret. </param>
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

    parser.AddErrorListener(new SyntaxErrorListener());
    parser.ErrorHandler = new SyntaxErrorStrategy();

    // Parse the tokens to generate the "Concrete Syntax Tree".
    var cst      = parser.startRule();
    var ast      = new ASTGenerator().GenerateASTFromCST(cst);
    var compiler = new Compiler();
    compiler.Status += (sender, args) => { Status?.Invoke(sender, args); };
    compiler.Compile(ast);
  }
}
