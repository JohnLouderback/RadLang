using System.Text;

namespace RadUtils;

public static class StringExtensions {
  /// <summary>
  ///   Provides a "Title Case" string, given a "PascalCase" string.
  /// </summary>
  /// <param name="str"> The source string. </param>
  /// <returns> The source string in "Title case" format. </returns>
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
