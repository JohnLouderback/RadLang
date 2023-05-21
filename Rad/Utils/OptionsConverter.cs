using System.ComponentModel;
using System.Globalization;
using RadUtils;

namespace Rad.Utils;

/// <summary>
///   This class is used to convert an enum to a string and back again. It is useful for the
///   conversation of command line arguments to enums and back again.
/// </summary>
/// <typeparam name="TOptions"> The enum to convert to and from. </typeparam>
public class OptionsConverter<TOptions> : EnumConverter {
  public OptionsConverter() : base(typeof(TOptions)) {}


  /// <summary>
  ///   Determines if the converter can convert from the given type. This converter can only
  ///   convert from a string to a given enum type.
  /// </summary>
  /// <param name="context"> Provides contextual information required for conversion. </param>
  /// <param name="sourceType"> The type to convert from. </param>
  /// <returns> Whether or not the converter can convert from the given type. </returns>
  public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
    return sourceType == typeof(string);
  }


  /// <summary>
  ///   Determines if the converter can convert to the given type. This converter can only
  ///   convert to a string from a given enum type.
  /// </summary>
  /// <param name="context"> Provides contextual information required for conversion. </param>
  /// <param name="destinationType"> The type to convert to. </param>
  /// <returns> Whether or not the converter can convert to the given type. </returns>
  public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
    return destinationType == typeof(string);
  }


  /// <summary>
  ///   Converts the given value to the given type. This converter can only convert from a
  ///   string to a given enum type.
  /// </summary>
  /// <param name="context"> Provides contextual information required for conversion. </param>
  /// <param name="culture">
  ///   The culture to use for conversion. In some cases, this is useful for
  ///   locale-dependent conversions.
  /// </param>
  /// <param name="value"> The value to convert into <typeparamref name="TOptions" />. </param>
  /// <returns> The converted value. </returns>
  public override object ConvertFrom(
    ITypeDescriptorContext context,
    CultureInfo culture,
    object value
  ) {
    if (value is string) {
      // Trim the value and capitalize the first letter of each word while lower casing the rest.
      // So "HelloWORLD" becomes "Helloworld". Enum members must be PascalCase with only the first
      // letter capitalized.
      var strValue = ((string)value).Trim().CapitalizeRestToLower();
      if (Enum.IsDefined(typeof(TOptions), strValue)) {
        return Enum.Parse(typeof(TOptions), strValue, true);
      }
    }

    return base.ConvertFrom(context, culture, value);
  }


  /// <summary>
  ///   Converts the given value to the given type. This converter can only convert to a
  ///   string from a given enum type.
  /// </summary>
  /// <param name="context"> Provides contextual information required for conversion. </param>
  /// <param name="culture">
  ///   The culture to use for conversion. In some cases, this is useful for locale-dependent
  ///   conversions.
  /// </param>
  /// <param name="value"> The <typeparamref name="TOptions" /> to convert into a string. </param>
  /// <param name="destinationType"> The type to convert to. </param>
  /// <returns> The converted value. </returns>
  public override object ConvertTo(
    ITypeDescriptorContext context,
    CultureInfo culture,
    object value,
    Type destinationType
  ) {
    if (destinationType == typeof(string) &&
        value is TOptions) {
      // Convert the value to a string and lower case it. So "HelloWorld" becomes "helloworld".
      return ((TOptions)value).ToString().ToLower();
    }

    return base.ConvertTo(context, culture, value, destinationType);
  }
}
