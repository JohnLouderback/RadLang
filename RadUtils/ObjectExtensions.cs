namespace RadUtils;

/// <summary>
///   Extension methods which apply to all objects.
/// </summary>
public static class ObjectExtensions {
  /// <summary>
  ///   Checks to see if the object has a property with the given name.
  /// </summary>
  /// <param name="obj"> The object to check. </param>
  /// <param name="propertyName"> The property name to check for in the object. </param>
  /// <returns> A boolean value indicating whether the property was found in the object. </returns>
  /// <example>
  ///   <code language="csharp">
  ///     var myObject = new { MyProperty = 1 };
  ///     var hasProperty = myObject.HasProperty("MyProperty"); // hasProperty == true
  ///   </code>
  /// </example>
  public static bool HasProperty(this object obj, string propertyName) {
    return obj.GetType().GetProperty(propertyName) != null;
  }


  /// <summary>
  ///   Checks to see if the object has a property with the given name. Also outputs the same object, but
  ///   cast to "dynamic" to allow for accessing the property dynamically. Useful for ternaries and other
  ///   expressions.
  /// </summary>
  /// <param name="obj"> The object to check. </param>
  /// <param name="propertyName"> The property name to check for in the object. </param>
  /// <param name="outObj"> The source object, but cast as "dynamic". </param>
  /// <returns> A boolean value indicating whether the property was found in the object. </returns>
  /// <example>
  ///   <code language="csharp">
  ///     var myObject = new { MyProperty = 1 };
  ///     var myProp = myObject.HasProperty("MyProperty", out var myObjectDynamic) ? myObjectDynamic.MyProperty : 0;
  ///   </code>
  /// </example>
  public static bool HasProperty(this object obj, string propertyName, out dynamic outObj) {
    outObj = obj;
    return obj.GetType().GetProperty(propertyName) != null;
  }
}
