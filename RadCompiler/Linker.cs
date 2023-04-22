namespace RadCompiler;

public class Linker {
  /// <summary>
  ///   <c> DetermineLinker </c> is a method that returns a linker that is suitable for the current operating system.
  ///   The method first checks for a linker in commonly used installation directories for the current platform,
  ///   and if it is unable to find one, it checks the PATH environment variable. If no suitable linker is found,
  ///   the method throws an exception.
  /// </summary>
  /// <returns>
  ///   A string containing the name of the linker that is suitable for the current operating system.
  /// </returns>
  /// <exception cref="Exception"> Thrown when no suitable linker is found. </exception>
  private string DetermineLinker() {
    var      linker            = "";
    string[] commonInstallDirs = {};
    string[] fallbackLinkers   = {};

    switch (Environment.OSVersion.Platform) {
      // Determine the common installation paths and linkers for the current platform.
      case PlatformID.Win32NT: {
        var paths = new[] {
          @":\cygwin64\bin\",
          @":\mingw64\bin\",
          @":\Windows\System32\"
        };

        var driveLetters = DriveInfo.GetDrives().Select(d => d.Name);
        // Map the above paths to each drive letter on the system.
        var drivePaths = paths.SelectMany(p => driveLetters.Select(d => d + p));
        // Sort drive "C:" first when looking for the executables.
        commonInstallDirs = drivePaths.OrderBy(p => p == @"C:\").ToArray();

        fallbackLinkers = new[] {
          "link.exe",
          "ld.exe",
          "clang.exe"
        };
        break;
      }
      case PlatformID.Unix:
        commonInstallDirs = new[] {
          "/usr/bin/",
          "/usr/local/bin/",
          "/usr/local/libexec/gcc/",
          "/usr/local/mingw/bin/"
        };
        fallbackLinkers = new[] {
          "ld",
          "gcc",
          "clang"
        };
        break;
    }

    // Check to see if any of the linkers exists in any of the common install directories for this OS. 
    foreach (var dir in commonInstallDirs) {
      foreach (var fallback in fallbackLinkers) {
        if (File.Exists(Path.Combine(dir, fallback))) {
          // If the linker exists, use it.
          return fallback;
        }
      }
    }

    // If no linker could be found in a common location, check for any of the linkers in any path in
    // the system's "PATH" environment variable.

    // check the PATH environment variable
    var path = Environment.GetEnvironmentVariable("PATH");
    // If the PATH environment variable is not null, check for the linkers in the paths. The path
    // variable is a string containing a list of paths separated by semicolons. It _should not_ be
    // null, but it is possible that it is, so we check for that.
    if (path is not null) {
      // Paths in the "PATH" environment variable.
      var pathPaths = path.Split(';');
      foreach (var fallback in fallbackLinkers) {
        foreach (var p in pathPaths) {
          if (File.Exists(Path.Combine(p, fallback))) {
            // If a linker was found in the PATH, use it.
            return fallback;
          }
        }
      }
    }

    throw new Exception("Unable to find a suitable linker for the current operating system.");
  }
}
