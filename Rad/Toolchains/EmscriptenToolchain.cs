using System.Diagnostics;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Rad.Utils;
using RadUtils;
using Spectre.Console;
using Progress = Rad.Commands.Progress;

namespace Rad.Toolchains;

public class EmscriptenToolchain : IBuildableToolchain<string, string>, IActivatableToolchain {
  private const string emVersion = "3.1.37";
  private bool? isInstalled;

  /// <summary>
  ///   Whether or not Emscripten is installed. This is ascertained by checking if the emsdk
  ///   directory exists relative to the application.
  /// </summary>
  public bool IsInstalled {
    get {
      // If we have not checked if the toolchain is installed, then we should check.
      if (isInstalled == null) {
        // If the user does not have the emsdk directory, then the toolchain is not installed.
        return (bool)(isInstalled = Directory.Exists(DirectoryUtils.MakeAbsolutePath("./emsdk")));
      }

      // Otherwise, we should just return the cached value.
      return (bool)isInstalled;
    }
  }

  /// <summary>
  ///   Whether or not Emscripten is activated.
  /// </summary>
  public bool IsActivated { get; private set; }


  /// <summary>
  ///   Activates the Emcripten toolchain. This is done by running the emsdk activate command along
  ///   with any of `emsdk_env.bat` or `emsdk_env.sh` that are present in the emsdk directory.
  /// </summary>
  /// <returns> Whether or not the toolchain was activated successfully. </returns>
  public async Task<bool> Activate() {
    Logging.Info("Activating Emscripten toolchain.");

    // Suppress the "EMSDK environment variable set" message.
    Environment.SetEnvironmentVariable("EMSDK_QUIET", "1", EnvironmentVariableTarget.Process);

    // If the toolchain is not installed, then we cannot activate it.
    if (!IsInstalled) {
      Logging.Error("Cannot activate Emscripten toolchain because it is not installed.");
      return false;
    }

    // If the toolchain is already activated, then we don't need to activate it again.
    if (IsActivated) {
      Logging.Info("Emscripten toolchain is already activated.");
      return true;
    }

    // Get the emsdk directory.
    var emscriptenDir = DirectoryUtils.MakeAbsolutePath("./emsdk/emsdk-main");

    // Check if we're in a Unix-like environment.
    if (Environment.OSVersion.Platform == PlatformID.Unix ||
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
      var emsdkEnv = DirectoryUtils.NormalizePath(Path.Combine(emscriptenDir, "./emsdk_env.sh"));
      // Check if the "emsdk_env.sh" file exists.
      if (File.Exists(Path.Combine(emscriptenDir, "./emsdk_env.sh"))) {
        Logging.Info($"Running \"sh {emsdkEnv}\".");
        // Run the "emsdk_env.sh" file.
        await ProcessUtils.RunCommandAsync(
            "sh",
            emsdkEnv,
            false,
            false,
            emscriptenDir
          );
      }
      // Otherwise, the "emsdk_env.sh" file does not exist.
      else {
        Logging.Error(
            "\"emsdk_env.sh\" does not exist. Cannot activate Emscripten toolchain."
          );
        return false;
      }
    }
    // Otherwise, we're in a Windows environment.
    else {
      var emsdkEnvBat =
        DirectoryUtils.NormalizePath(Path.Combine(emscriptenDir, "./emsdk_env.bat"));
      // Check if the "emsdk_env.bat" file exists.
      if (File.Exists(emsdkEnvBat)) {
        Logging.Info($"Running \"cmd /c {emsdkEnvBat}\"");
        // Run the "emsdk_env.bat" file.
        await ProcessUtils.RunCommandAsync(
            "cmd",
            $"/c {emsdkEnvBat}",
            false,
            false,
            emscriptenDir
          );
      }
      // Otherwise, the "emsdk_env.bat" file does not exist.
      else {
        Logging.Error(
            "\"emsdk_env.bat\" does not exist. Cannot activate Emscripten toolchain."
          );
        return false;
      }
    }

    // If we made it this far, then the toolchain was activated successfully.
    IsActivated = true;
    return true;
  }


  public async Task<bool> HandOffBuild(string pathToIRFile, Action<string> result) {
    Logging.Info("Handing off build to Emscripten toolchain.");

    // If the toolchain is not installed, then we cannot hand off the build.
    if (!IsInstalled) {
      Logging.Error("Cannot hand off build to Emscripten toolchain because it is not installed.");
      return false;
    }

    // If the toolchain is not activated, then we cannot hand off the build.
    if (!IsActivated) {
      Logging.Error("Cannot hand off build to Emscripten toolchain because it is not activated.");
      return false;
    }

    // Get the emscripten directory. 
    var emscriptenDir = DirectoryUtils.MakeAbsolutePath("./emsdk/emsdk-main");
    // Get the path to the static runtime library for linking with the IR file.
    var staticRuntimeLibrary =
      DirectoryUtils.MakeAbsolutePath("./libs/toolchains/emcc/libRadLib.a");

    // Get the file extension for running any binaries. This is empty on Unix-like systems and
    // ".bat" on Windows. This is because on Unix-like systems, we can run binaries directly, but on
    // Windows, we need to run them through scripts on the command prompt.
    var binExtension = Environment.OSVersion.Platform == PlatformID.Unix ||
                       RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                       RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                         ? ""
                         : ".bat";

    // Get the path to the emcc executable.
    var emccPath = DirectoryUtils.NormalizePath(
        Path.Combine(emscriptenDir, $"./upstream/emscripten/emcc{binExtension}")
      );

    Logging.Info($"Running emcc at \"{emccPath}\".");
    var emccResult = await ProcessUtils.RunCommandAsync(
                         emccPath,
                         $"-s {pathToIRFile} {staticRuntimeLibrary} -o {Path.GetFileNameWithoutExtension(pathToIRFile)}.html",
                         false,
                         false,
                         Path.GetDirectoryName(pathToIRFile)
                       );

    if (emccResult.ExitCode != 0) {
      Logging.Error(
          $"Failed to hand off build to Emscripten toolchain. emcc exited with code {emccResult.ExitCode}."
        );
      return false;
    }

    Logging.Success("Successfully handed off build to Emscripten toolchain. Build was successful.");

    result("");

    return true;
  }


  public async Task<bool> Install() {
    // If the user does not already have the emsdk, then we should download it.
    if (!IsInstalled) {
      Logging.Info("Downloading Emscripten SDK...");
      await AnsiConsole.Progress()
        .StartAsync(
            async ctx => {
              // Define tasks
              var progress = new Progress();
              var task1    = ctx.AddTask("[green]Downloading Emscripten SDK.[/]");
              FileUtils.DownloadFileAsync(
                  "https://github.com/emscripten-core/emsdk/archive/refs/heads/main.zip",
                  "emsdk.zip",
                  progress
                );

              while (!ctx.IsFinished) {
                // Re-check every 60 frames.
                await Task.Delay(16);

                // While the progress is at 0, the task is indeterminate. Once it begins progressing
                // it is no longer indeterminate.
                task1.IsIndeterminate = progress.Value == 0;

                // Update the progress
                task1.Value = progress.Value;
              }
            }
          );
      Logging.Info("Extracting Emscripten SDK...");

      // Decompress the emsdk.zip file.
      ZipFile.ExtractToDirectory("./emsdk.zip", "./emsdk");
    }
    // Otherwise, the user already has the emsdk.
    else {
      Logging.Info(
          "Existing Emscripten SDK installation found. Updating Emscripten SDK..."
        );
    }

    Logging.Info("Installing Emscripten SDK...");
    var    emscriptenDir = DirectoryUtils.MakeAbsolutePath("./emsdk/emsdk-main");
    string process;
    // Check if we're in a Unix-like environment.
    if (Environment.OSVersion.Platform == PlatformID.Unix ||
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
      process = DirectoryUtils.NormalizePath(Path.Combine(emscriptenDir, "./emsdk"));
      // Make the emsdk executable
      Process.Start("chmod", "+x ./emsdk/emsdk");
    }
    // Otherwise, we're in a Windows environment.
    else {
      process = DirectoryUtils.NormalizePath(Path.Combine(emscriptenDir, "./emsdk.bat"));
    }

    Logging.Info($"Executing: {process} install {emVersion}");
    // Run the emsdk
    var installResult =
      await ProcessUtils.RunCommandAsync(
          process,
          $"install {emVersion}",
          false,
          false,
          emscriptenDir
        );
    if (installResult.ExitCode != 0) {
      Logging.Error(
          $"install {emVersion} exited with code {installResult.ExitCode}. Install was unsuccessful."
        );
      return false;
    }

    Logging.Success(
        $"install {emVersion} exited with code {installResult.ExitCode}. Install was successful."
      );

    Logging.Info("Activating Emscripten SDK...");
    Logging.Info($"Executing: {process} activate {emVersion}");
    var activateResult =
      await ProcessUtils.RunCommandAsync(
          process,
          $"activate {emVersion}",
          false,
          false,
          emscriptenDir
        );

    if (activateResult.ExitCode != 0) {
      Logging.Error(
          $"activate {emVersion} exited with code {activateResult.ExitCode}. Activate was unsuccessful."
        );
      return false;
    }

    Logging.Success(
        $"activate latest exited with code {activateResult.ExitCode}. Activate was successful."
      );

    isInstalled = true;
    return true;
  }
}
