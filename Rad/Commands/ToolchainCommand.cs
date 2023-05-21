using Rad.Toolchains;
using Spectre.Console.Cli;

namespace Rad.Commands;

/// <summary>
///   Represents available toolchains for the compiler.
/// </summary>
public enum Toolchain {
  Emscripten
  // Wasmtime
}

internal class Progress : IProgress<int> {
  public int Value { get; private set; }


  public void Report(int value) {
    Value = value;
  }
}

public class ToolchainCommand : AsyncCommand<ToolchainCommand.Settings> {
  public override async Task<int> ExecuteAsync(
    CommandContext context,
    Settings settings
  ) {
    var toolchain = new EmscriptenToolchain();
    await toolchain.Install();
    return 0;
  }


  public class Settings : CommandSettings {
    [CommandArgument(0, "<tool>")] public Toolchain Toolchain { get; set; }
  }
}
