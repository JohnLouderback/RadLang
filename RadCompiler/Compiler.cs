using LLVMSharp;
using LLVMSharp.Interop;

namespace RadCompiler;

public class Compiler {
  public unsafe void Compile() {
    // Creates the application. A module is the whole program, not just a single file. 
    var module = LLVMModuleRef.CreateWithName("my cool jit");
    // Creates a context, such as for a thread.
    var context = LLVMContextRef.Create();
    // Creates a "builder" for generating IR code.
    var builder = LLVMBuilderRef.Create(context);
  }
}
