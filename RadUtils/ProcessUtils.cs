using System.Diagnostics;

namespace RadUtils;

public record ProcessResult(int ExitCode, string Output);

/// <summary>
///   Utilities for running processes and commands more easily.
/// </summary>
public static class ProcessUtils {
  /// <summary>
  ///   Runs a command asynchronously and returns the exit code and output. If
  ///   <paramref name="redirectOutput" /> is <c> true </c>, the output will be redirected to the
  ///   console as well.
  /// </summary>
  /// <param name="fileName"> The name of the executable to run. </param>
  /// <param name="arguments"> The arguments to pass to the executable. </param>
  /// <param name="redirectOutput">
  ///   Whether or not to redirect the output to the console. Only meaningful when the
  ///   <paramref name="captureOutput" /> parameter is <c> true </c>. This allows us to continue to
  ///   output the output of the process to the console while still capturing it.
  /// </param>
  /// <param name="captureOutput">
  ///   Whether or not to capture the output in the return object. If this is <c> false </c>, the
  ///   output will not be captured and will not be available in the return object. Additionally,
  ///   <paramref name="redirectOutput" /> will be ignored if this is <c> false </c>, and the
  ///   output of the process will be output to the console as normal.
  /// </param>
  /// <param name="workingDirectory"> The working directory to run the command in. </param>
  /// <returns> The exit code and output of the process. </returns>
  public static async Task<ProcessResult> RunCommandAsync(
    string fileName,
    string arguments,
    bool redirectOutput = true,
    bool captureOutput = true,
    string workingDirectory = ""
  ) {
    // If the working directory is relative, make it absolute to the app's directory.
    if (DirectoryUtils.IsPathRelative(workingDirectory)) {
      workingDirectory =
        DirectoryUtils.EnsureTrailingSlash(DirectoryUtils.MakeAbsolutePath(workingDirectory));
    }

    var process = new Process {
      StartInfo = new ProcessStartInfo {
        FileName               = fileName,
        Arguments              = arguments,
        WorkingDirectory       = workingDirectory,
        UseShellExecute        = false,
        RedirectStandardOutput = captureOutput,
        RedirectStandardError  = captureOutput
      }
    };

    var output = new StringWriter();

    // If we're capturing the output, we need to set up the event handlers to capture the output
    // and write it to the output string.
    if (captureOutput) {
      process.OutputDataReceived += (sender, e) => {
        if (e.Data == null) return;
        output.WriteLine(e.Data);
        // If we're redirecting the output, we need to write it to the console as well.
        if (redirectOutput) Console.WriteLine(e.Data);
      };

      process.ErrorDataReceived += (sender, e) => {
        if (e.Data == null) return;
        output.WriteLine(e.Data);
        // If we're redirecting the output, we need to write it to the console as well.
        if (redirectOutput) Console.WriteLine(e.Data);
      };
    }

    process.Start();

    // If we're capturing the output, we need to start reading the output asynchronously.
    if (captureOutput) {
      process.BeginOutputReadLine();
      process.BeginErrorReadLine();
    }

    await process.WaitForExitAsync();

    return new ProcessResult(process.ExitCode, output.ToString());
  }
}
