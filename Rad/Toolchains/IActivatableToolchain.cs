namespace Rad.Toolchains;

/// <summary>
///   The <c> IActivatableToolchain </c> interface is the base interface for all toolchains in
///   Rad that can be activated. Activation is the process of making the toolchain available for
///   use within the Rad compiler.
/// </summary>
public interface IActivatableToolchain : IToolchain {
  /// <summary>
  ///   Gets a value indicating whether this toolchain is currently activated.
  /// </summary>
  bool IsActivated { get; }


  /// <summary>
  ///   Activates this toolchain.
  /// </summary>
  /// <returns>
  ///   <c> true </c> if the toolchain was activated successfully; otherwise, <c> false </c>.
  /// </returns>
  Task<bool> Activate();
}
