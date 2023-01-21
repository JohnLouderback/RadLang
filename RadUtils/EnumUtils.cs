namespace RadUtils;

public class EnumUtils {
  /// <summary>
  ///   Dynamically reads the value of an enum type's member.
  /// </summary>
  /// <param name="enumConst"> The "key" in the enum to read the value of. </param>
  /// <typeparam name="T"> The enum to read. </typeparam>
  /// <typeparam name="TNumericType"> A type to cast the result to, such as <c> int </c> </typeparam>
  /// <returns> The value of <c> T.enumConst </c> </returns>
  /// <exception cref="NotSupportedException"> Thrown when <c> enumConst </c> can't be found in <c> T </c> </exception>
  public static TNumericType GetValueOf<T, TNumericType>(string enumConst) {
    var enumType = typeof(T);

    if (enumType is null) {
      throw new NotSupportedException(
          $"Specified enum type \"{enumConst}\" could not be found"
        );
    }

    return (TNumericType)Enum.Parse(enumType, enumConst);
  }


  /// <summary>
  ///   Dynamically reads the value of an enum type's member.
  /// </summary>
  /// <param name="enumConst"> The "key" in the enum to read the value of. </param>
  /// <typeparam name="T"> The enum to read. </typeparam>
  /// <returns> The value of <c> T.enumConst </c> </returns>
  /// <exception cref="NotSupportedException"> Thrown when <c> enumConst </c> can't be found in <c> T </c> </exception>
  public static T GetValueOf<T>(string enumConst) {
    return GetValueOf<T, T>(enumConst);
  }
}
