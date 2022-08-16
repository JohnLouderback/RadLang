namespace RadCompiler.Utils;

public enum LLVMResultType {
  Success,
  Error
}

/// <summary>
///   The "LLVM Result" exception exists as a way to handle results that are provided by the LLVM
///   toolchain that we may need to handle upstream of the compiler. An example mgiht be if a function
///   or module validation fails, or if compilation is successful. Rather than strings, LLVM often
///   outputs to stdout. This class allows us to handle those more gracefully by way of the
///   `GetDetails` and `GetMessage` properties.
/// </summary>
public class LLVMResult : CException {
  public LLVMResultType ResultType { get; }

  public Action? GetDetails { get; }


  public LLVMResult(LLVMResultType resultType, Action getMessage) : this(
      resultType,
      getMessage,
      getDetails: null
    ) {}


  public LLVMResult(LLVMResultType resultType, Action getMessage, Action? getDetails) : base(
      getMessage
    ) {
    ResultType = resultType;
    GetDetails = getDetails;
  }
}
