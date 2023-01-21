namespace RadTypeChecker.TypeErrors;

/// <summary>
///   Represents the numeric representations of all known type errors.
/// </summary>
public enum TypeErrors : uint {
  /// <summary> See: <see cref="UndefinedReferenceError" /> </summary>
  UndefinedReferenceError = 1,

  /// <summary> See: <see cref="IncorrectNumberOfArgumentsError" /> </summary>
  IncorrectNumberOfArgumentsError = 2
}
