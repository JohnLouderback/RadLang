namespace Rad.Toolchains;

public interface IBuildableToolchain<TBuildDataInput, TBuildDataOutput> : IToolchain {
  /// <summary>
  ///   Hands off the build to the toolchain. This is where the toolchain will take over and
  ///   continue the build process.
  /// </summary>
  /// <param name="requiredData">
  ///   The required data for the toolchain to continue the build. This could be nothing or it
  ///   could be data from the previous toolchain or step in the build process needed to continue
  ///   the build.
  /// </param>
  /// <param name="result">
  ///   The result of the build. This could be nothing or it could be data to be passed to the
  ///   next toolchain or step in the build process.
  /// </param>
  /// <returns>
  ///   The result of the build.
  /// </returns>
  Task<bool> HandOffBuild(TBuildDataInput requiredData, Action<TBuildDataOutput> result);
}
