using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace RadParser.Utils;

public static class ParserExtensions {
  public static string GetFullText(this ParserRuleContext context) {
    if (context.Start == null ||
        context.Stop == null ||
        context.Start.StartIndex < 0 ||
        context.Stop.StopIndex < 0) {
      return context.GetText(); // Fallback
    }

    var range = Interval.Of(context.Start.StartIndex, context.Stop.StopIndex);

    // If the range is zero, something is amiss. Return an empty string.
    if (range.Length == 0) return "";

    return context.Start.InputStream.GetText(range);
  }
}
