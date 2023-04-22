using System.Text;

namespace RadCompiler.Utils;

/// <summary>
///   A collection of utilities for working with native code. Particularly for navigating between
///   native types and managed types.
/// </summary>
public static class NativeUtils {
  /// <summary>
  ///   Converts a string to a pointer to a null-terminated array of bytes - a C-style string.
  /// </summary>
  /// <param name="str"> The string to convert from. </param>
  /// <returns> A native pointer to a byte array ending in the \0 null character. </returns>
  public static unsafe sbyte* ToPointer(this string str) {
    // Space for terminating \0
    var bytes = new byte[Encoding.Default.GetByteCount(str) + 1];
    Encoding.Default.GetBytes(str, 0, str.Length, bytes, 0);

    fixed (byte* b = bytes) {
      var b2 = (sbyte*)b;
      return b2;
    }
  }
}
