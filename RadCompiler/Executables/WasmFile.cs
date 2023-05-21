using LLVMSharp.Interop;
using RadParser.AST.Node;
using RadUtils;

namespace RadCompiler;

public class WasmFile : Executable {
  public string IRFilePath { get; private set; }


  public override LLVMValueRef Build(Module ast) {
    // Build the AST.
    var entryPoint = base.Build(ast);

    // Output the LLVM IR to a file.
    if (RootModule is LLVMModuleRef rootModule) {
      var llvmIR   = rootModule.PrintToString();
      var fileName = DirectoryUtils.MakeAbsolutePath("./test.ll");
      File.WriteAllText(fileName, llvmIR);
      IRFilePath = fileName;
    }

    return entryPoint;
  }


  public override void Run() {
    throw new NotImplementedException();
  }


  protected override void Initialize() {
    // Initialize LLVM for WebAssembly this is essentially the same as:
    // `clang -emit-llvm --target=wasm32-unknown-unknown-elf`
    LLVM.InitializeWebAssemblyTargetInfo();
    LLVM.InitializeWebAssemblyTarget();
    LLVM.InitializeWebAssemblyAsmPrinter();
    LLVM.InitializeWebAssemblyDisassembler();
  }
}
