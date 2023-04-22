namespace RadUtils;

/// <summary>
///   A collection of utilities for working with enums more easily.
/// </summary>
public static class EnumUtils {
  /// <summary>
  ///   Dynamically reads the value of an enum type's member.
  /// </summary>
  /// <param name="enumConst"> The "key" in the enum to read the value of. </param>
  /// <typeparam name="T"> The enum to read. </typeparam>
  /// <typeparam name="TNumericType"> A type to cast the result to, such as <c> int </c> </typeparam>
  /// <returns> The value of <c> T.enumConst </c> </returns>
  /// <exception cref="NotSupportedException"> Thrown when <c> enumConst </c> can't be found in <c> T </c> </exception>
  /// <example>
  ///   <code language="csharp">
  ///     var myEnum = EnumUtils.GetValueOf&lt;MyEnum, int&gt;("MyEnumValue"); // myEnum == 1
  ///   </code>
  /// </example>
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
  /// <example>
  ///   <code language="csharp">
  ///     var myEnum = EnumUtils.GetValueOf&lt;MyEnum&gt;("MyEnumValue"); // myEnum == MyEnum.MyEnumValue
  ///   </code>
  /// </example>
  public static T GetValueOf<T>(string enumConst) {
    return GetValueOf<T, T>(enumConst);
  }
}
