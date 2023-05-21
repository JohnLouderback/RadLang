namespace Rad.Toolchains;

public abstract class ToolchainFactory {
  /// <summary>
  ///   The dictionary of toolchains that have already been instantiated. Used to ensure that
  ///   toolchains are singletons.
  /// </summary>
  private static readonly Dictionary<Type, IToolchain> toolchains = new();


  /// <summary>
  ///   Gets the singleton instance of this toolchain. If the toolchain has not been instantiated
  ///   yet, it will be instantiated and added to the dictionary cached of toolchains.
  /// </summary>
  /// <typeparam name="T">
  ///   The type of the toolchain to get.
  /// </typeparam>
  public static T GetToolchain<T>() where T : class, IToolchain, new() {
    if (toolchains.TryGetValue(typeof(T), out var outToolchain)) {
      return (T)outToolchain;
    }

    var newToolChain = new T();
    toolchains.Add(typeof(T), newToolChain);
    return new T();
  }
}
