using LLVMSharp.Interop;

namespace RadCompiler.Utils;

public class LLVMUtils {
  public static unsafe bool LoadLibraryPermanently(string Filename) {
    /*NativeLibrary.Load(
        "RadLib.dll",
        Assembly.GetExecutingAssembly(),
        DllImportSearchPath.ApplicationDirectory
      );*/
    var marshaledString = (sbyte*)new MarshaledString(Filename);
    return LLVM.LoadLibraryPermanently(marshaledString) > 0;
  }
}
