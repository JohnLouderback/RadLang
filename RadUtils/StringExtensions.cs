using System.Text;

namespace RadUtils;

public static class StringExtensions {
  /// <summary>
  ///   Provides a string wherein the first letter is capitalized, and the rest are lower case.
  /// </summary>
  /// <param name="str"> The source string. </param>
  /// <returns>
  ///   This source string with the first letter capitalized, and the rest lower case.
  /// </returns>
  /// <example>
  ///   <code language="csharp">
  ///   var capitalized = "pascalCase".CapitalizeRestToLower(); // capitalized == "Pascalcase"
  ///   var capitalized = "PascalCase".CapitalizeRestToLower(); // capitalized == "Pascalcase"
  ///   var capitalized = "PASCALCASE".CapitalizeRestToLower(); // capitalized == "Pascalcase"
  ///   </code>
  /// </example>
  public static string CapitalizeRestToLower(this string str) {
    // If the string is null or empty, return it as-is.
    if (string.IsNullOrEmpty(str)) {
      return str;
    }

    // Grab the first character, capitalize it, and append the rest of the string, lower-cased.
    return str[..1].ToUpper() + str.Substring(1).ToLower();
  }


  /// <summary>
  ///   Provides a "camelCase" string, given a "PascalCase" string.
  /// </summary>
  /// <param name="str"> The source string. </param>
  /// <returns> The source string in "camelCase" format. </returns>
  /// <example>
  ///   <code language="csharp">
  ///     var camelCase = "PascalCase".PascalCaseToCamelCase(); // camelCase == "pascalCase"
  ///   </code>
  /// </example>
  public static string PascalCaseToCamelCase(this string str) {
    return str[0].ToString().ToLower() + str[1..];
  }


  /// <summary>
  ///   Provides a "Title Case" string, given a "PascalCase" string.
  /// </summary>
  /// <param name="str"> The source string. </param>
  /// <returns> The source string in "Title case" format. </returns>
  /// <example>
  ///   <code language="csharp">
  ///     var titleCase = "PascalCase".PascalCaseToTitleCase(); // titleCase == "Pascal Case"
  ///   </code>
  /// </example>
  public static string PascalCaseToTitleCase(this string str) {
    var newStr = new StringBuilder();

    for (var i = 0; i < str.Length; i++) {
      var c = str[i];
      // If the current character is uppercase and not first character, then prepend a space to it.
      if (char.IsUpper(c) &&
          i > 0) {
        newStr.Append(' ');
      }

      newStr.Append(c);
    }

    return newStr.ToString();
  }
}
