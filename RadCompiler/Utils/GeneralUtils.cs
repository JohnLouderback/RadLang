using System.Reflection;

namespace RadCompiler.Utils;

public static class GeneralUtils {
  public static string GetApplicationDirectory() {
    var strExeFilePath = Assembly.GetExecutingAssembly().Location;
    return Path.GetDirectoryName(strExeFilePath);
  }
}
