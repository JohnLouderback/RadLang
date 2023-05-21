namespace Rad.Toolchains;

/// <summary>
///   The <c> IToolchain </c> interface is the base interface for all toolchains in the Rad.
/// </summary>
public interface IToolchain {
  /// <summary>
  ///   Gets a value indicating whether this toolchain is installed.
  /// </summary>
  bool IsInstalled { get; }


  /// <summary>
  ///   Installs this toolchain to the user's system.
  /// </summary>
  /// <returns>
  ///   <c> true </c> if the toolchain was installed successfully; otherwise, <c> false </c>.
  /// </returns>
  Task<bool> Install();
}
