using System.Text;

namespace RadCompiler.Utils;

public static class NativeUtils {
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
