namespace RadUtils;

/// <summary>
///   A collection of utilities for working with directories more easily.
/// </summary>
public static class DirectoryUtils {
  /// <summary>
  ///   Ensures that a provided path ends with a platform appropriate trailing slash, if it does not
  ///   already.
  /// </summary>
  /// <param name="path"> The path to ensure ends with a trailing slash. </param>
  /// <returns> The path with a trailing slash. </returns>
  /// <see href="https://stackoverflow.com/a/20406065" />
  /// <example>
  ///   <code lang="csharp">
  ///   DirectoryUtils.EnsureTrailingSlash("./some/path"); // "./some/path/"
  ///   DirectoryUtils.EnsureTrailingSlash("./some/path/"); // "./some/path/"
  ///   </code>
  /// </example>
  public static string EnsureTrailingSlash(string path) {
    // They're always one character but EndsWith is shorter than
    // array style access to last path character. Change this
    // if performance are a (measured) issue.
    var separator1 = Path.DirectorySeparatorChar.ToString();
    var separator2 = Path.AltDirectorySeparatorChar.ToString();

    // Trailing white spaces are always ignored but folders may have
    // leading spaces. It's unusual but it may happen. If it's an issue
    // then just replace TrimEnd() with Trim(). Tnx Paul Groke to point this out.
    path = path.TrimEnd();

    // Argument is always a directory name then if there is one
    // of allowed separators then I have nothing to do.
    if (path.EndsWith(separator1) ||
        path.EndsWith(separator2)) {
      return path;
    }

    // If there is the "alt" separator then I add a trailing one.
    // Note that URI format (file://drive:\path\filename.ext) is
    // not supported in most .NET I/O functions then we don't support it
    // here too. If you have to then simply revert this check:
    // if (path.Contains(separator1))
    //     return path + separator1;
    //
    // return path + separator2;
    if (path.Contains(separator2)) {
      return path + separator2;
    }

    // If there is not an "alt" separator I add a "normal" one.
    // It means path may be with normal one or it has not any separator
    // (for example if it's just a directory name). In this case I
    // default to normal as users expect.
    return path + separator1;
  }


  /// <summary>
  ///   Obtains the path to the currently running executable - whatever application is including the
  ///   compiler library.
  /// </summary>
  /// <returns> A string of the path to the currently running executable. </returns>
  public static string GetApplicationDirectory() {
    var strExeFilePath = AppContext.BaseDirectory; //Assembly.GetExecutingAssembly().Location;
    return Path.GetDirectoryName(strExeFilePath) ??
           throw new InvalidOperationException(
               "Could not determine the current application's directory."
             );
  }


  public static bool IsPathRelative(string path) {
    // If we're on a Unix system, we can use the built-in Path.IsPathRooted method. This is because
    // Unix systems use a forward slash as the root directory, so if the path starts with a forward
    // slash, it's absolute. On Windows, however, the root directory is a drive letter, so we have
    // to do a little more work. For instance, on Windows, "\foo" is considered relative, but on
    // Unix, "/foo" is considered absolute.
    return Environment.OSVersion.Platform == PlatformID.Unix
             ? !Path.IsPathRooted(path)
             : !Path.IsPathFullyQualified(path);
  }


  /// <summary>
  ///   Takes a relative path and makes it absolute relative to the application directory.
  /// </summary>
  /// <param name="relativePath"> The relative path to make absolute. </param>
  /// <returns> The absolute path. </returns>
  public static string MakeAbsolutePath(string relativePath) {
    // Combines the relative path provided with the application directory path and then normalizes
    // the output so the relative segments and path separators are resolved and correct for the
    // current platform.
    return Path.GetFullPath(Path.Combine(GetApplicationDirectory(), relativePath));
  }


  /// <summary>
  ///   Normalizes a path to the current platform's path separator.
  /// </summary>
  /// <param name="path"> The path to normalize. </param>
  /// <returns> The normalized path. </returns>
  /// <example>
  ///   <code lang="csharp">
  ///   DirectoryUtils.NormalizePath("./some\\path"); // "./some/path"
  ///   DirectoryUtils.NormalizePath("./some/path"); // "./some/path"
  ///   </code>
  /// </example>
  public static string NormalizePath(string path) {
    return IsPathRelative(path)
             // If the path is relative, we can just replace the path separators with the current
             // platform's path separator.
             ? path.Replace('\\', Path.DirectorySeparatorChar)
               .Replace('/', Path.DirectorySeparatorChar)
             // If the path is absolute, we need to get the full path to normalize the path separators
             // and resolve any relative segments.
             : Path.GetFullPath(path);
  }
}
