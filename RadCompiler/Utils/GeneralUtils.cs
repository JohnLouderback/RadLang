using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RadCompiler.Utils;

/// <summary>
///   General utilities that are used throughout the compiler.
/// </summary>
public static class GeneralUtils {
  /// <summary>
  ///   Takes a timer that can be best expressed in either seconds or milliseconds and outputs a string
  ///   using the best candidate. If the timer is greater than 1 second, the string is output in seconds rather than
  ///   milliseconds.
  /// </summary>
  public static string GenerateTimeSpanString(Stopwatch timer) {
    string totalTimeString;
    totalTimeString = timer.Elapsed.TotalMilliseconds > 1000
                        ? $"{timer.Elapsed.TotalSeconds.ToString()} seconds"
                        : $"{timer.Elapsed.TotalMilliseconds.ToString()} ms";

    return totalTimeString;
  }


  /// <summary>
  ///   Obtains the path to the currently running executable - whatever application is including the
  ///   compiler library.
  /// </summary>
  /// <returns> A string of the path to the currently running executable. </returns>
  public static string GetApplicationDirectory() {
    var strExeFilePath = AppContext.BaseDirectory; //Assembly.GetExecutingAssembly().Location;
    return Path.GetDirectoryName(strExeFilePath);
  }


  /// <summary>
  ///   Gets the correct dynamic runtime library for the current platform. The dynamic runtime
  ///   library is the library that contains important "helper" functions that can be loaded into
  ///   this assembly for running Rad code as a script.
  /// </summary>
  /// <returns> A string of the filename of the correct runtime library. </returns>
  /// <exception cref="PlatformNotSupportedException">
  ///   Thrown when the platform could not be determined or was of an unexpected value.
  /// </exception>
  public static string GetPlatformSpecificDynamicRuntimeLib() {
    var os = Environment.OSVersion.Platform;
    switch (os) {
      case PlatformID.Win32NT:
      case PlatformID.Win32S:
      case PlatformID.Win32Windows:
      case PlatformID.WinCE:
        return "RadLib.dll";

      case PlatformID.Unix:
        return RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "libRadLib.dylib" : "libRadLib.so";

      case PlatformID.Other:
      case PlatformID.Xbox:
      default:
        throw new PlatformNotSupportedException(
            "Could not determine platform for Rad dynamic runtime library."
          );
    }
  }
}
