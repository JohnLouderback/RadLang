using System.Collections.Immutable;

namespace RadLanguageServerV2.Constructs;

/// <summary>
///   The <c> SemanticTokensDocument </c> class represents the semantic tokens for a document. It
///   contains the total length of the tokens, and the tokenized data representation of the tokens.
///   It is useful for building up a document's semantic tokens incrementally while visiting the
///   document's abstract syntax tree.
/// </summary>
public class SemanticTokensDocument {
  private int previousLine;
  private int previousColumn;

  /// <summary>
  ///   The total length of the collective tokens represented in this document.
  /// </summary>
  public int Length { get; private set; }

  /// <summary>
  ///   The tokenized data representation of the semantic tokens for this document.
  /// </summary>
  public ImmutableArray<int>.Builder Data { get; } = ImmutableArray<int>.Empty.ToBuilder();


  /// <summary>
  ///   Adds a new semantic token to the existing document.
  /// </summary>
  /// <param name="line"> The 0-based line number to token lives on. </param>
  /// <param name="char"> The 0-based column number the token's first character is placed in. </param>
  /// <param name="length"> The number of characters in the token. </param>
  /// <param name="tokenType"> The token's type. See: <see cref="SemanticTokenType" /> </param>
  /// <param name="tokenModifiers"> The token's modifier(s). See: <see cref="SemanticTokenModifier" /> </param>
  public void PushToken(
    int line,
    int @char,
    int length,
    SemanticTokenType tokenType,
    IEnumerable<SemanticTokenModifier> tokenModifiers
  ) {
    PushToken(
        line,
        @char,
        length,
        (byte)tokenType,
        tokenModifiers.Select(tokenMod => (ushort)tokenMod).ToArray()
      );
  }


  /// <summary>
  ///   Adds a new semantic token to the existing document.
  /// </summary>
  /// <param name="line"> The 0-based line number to token lives on. </param>
  /// <param name="char"> The 0-based column number the token's first character is placed in. </param>
  /// <param name="length"> The number of characters in the token. </param>
  /// <param name="tokenType">
  ///   The integral representation of the token's type. See: <see cref="SemanticTokenType" />
  /// </param>
  /// <param name="tokenModifiers">
  ///   The integral representation of the token's modifier(s). See: <see cref="SemanticTokenModifier" />
  /// </param>
  public void PushToken(
    int line,
    int @char,
    int length,
    byte tokenType,
    IEnumerable<ushort> tokenModifiers
  ) {
    ushort tokenModifiersFlags = 0b0000_0000;

    // Bitwise "OR" all the token modifiers together.
    foreach (var tokenModifier in tokenModifiers) {
      tokenModifiersFlags |= tokenModifier;
    }

    // Pass the "OR"d token modifier flags into the overload.
    PushToken(line, @char, length, tokenType, tokenModifiersFlags);
  }


  /// <summary>
  ///   Adds a new semantic token to the existing document.
  /// </summary>
  /// <param name="line"> The 0-based line number to token lives on. </param>
  /// <param name="char"> The 0-based column number the token's first character is placed in. </param>
  /// <param name="length"> The number of characters in the token. </param>
  /// <param name="tokenType">
  ///   The integral representation of the token's type. See: <see cref="SemanticTokenType" />
  /// </param>
  /// <param name="tokenModifiers">
  ///   The integral representation of the token's modifier(s). See: <see cref="SemanticTokenModifier" />
  /// </param>
  public void PushToken(int line, int @char, int length, byte tokenType, ushort tokenModifiers) {
    var num1 = line;
    var num2 = @char;

    // The values stored in the data are deltas, so they're relative to the previous lines and
    // columns. To get the correct delta values, we subtract the line and column numbers from the
    // previous values.
    if (Length > 0) {
      num1 -= previousLine;
      if (num1 == 0) {
        num2 -= previousColumn;
      }
    }

    Length += 5;
    Data.AddRange(num1, num2, length, tokenType, tokenModifiers);
    previousLine   = line;
    previousColumn = @char;
  }
}
