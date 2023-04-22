using LLVMSharp.Interop;

namespace RadCompiler.Utils;

/// <summary>
///   A collection of utilities for working with LLVM by abstracting over common operations.
/// </summary>
public class LLVMUtils {
  /// <summary>
  ///   Loads a library into the current process. This is used to load the RadLib library, which
  ///   contains the Rad runtime library. This library is necessary to load in at runtime so that
  ///   the JIT can find the Rad runtime functions. It is not necessary for AOT compilation as
  ///   the Rad runtime library is linked into the executable statically in AOT compilation.
  /// </summary>
  /// <param name="Filename"> The path to the library to load. </param>
  /// <returns> <c> true </c> if the library was loaded successfully, <c> false </c> otherwise. </returns>
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
