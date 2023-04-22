namespace RadCompiler.Utils;

/// <summary>
///   Gets the path to the currently running executable. <c> RadCompiler </c> is a library, so the executable could be any
///   application that includes it.
/// </summary>
public static class GeneralUtils {
  public static string GetApplicationDirectory() {
    var strExeFilePath = AppContext.BaseDirectory; //Assembly.GetExecutingAssembly().Location;
    return Path.GetDirectoryName(strExeFilePath);
  }
}
