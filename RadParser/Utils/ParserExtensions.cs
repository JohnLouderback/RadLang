using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace RadParser.Utils;

/// <summary>
///   A collection of utilities for working with ANTLR parsers. These are generally used to abstract
///   over common operations that can be performed on parser classes for convenience.
/// </summary>
public static class ParserExtensions {
  /// <summary>
  ///   Returns the full text of the given context. This is useful for getting the full text of a
  ///   context that spans multiple lines.
  /// </summary>
  /// <param name="context"> The context to get the full text of. </param>
  /// <returns> The full text of the given context. </returns>
  public static string GetFullText(this ParserRuleContext context) {
    // If we will be unable to get a valid range, return the text of the context as normal.
    if (context.Start == null ||
        context.Stop == null ||
        context.Start.StartIndex < 0 ||
        context.Stop.StopIndex < 0) {
      return context.GetText(); // Fallback
    }

    // Get the range of the context.
    var range = Interval.Of(context.Start.StartIndex, context.Stop.StopIndex);

    // If the range is zero, something is amiss. Return an empty string.
    if (range.Length == 0) return "";

    // Return the text of the context for the given range.
    return context.Start.InputStream.GetText(range);
  }
}
