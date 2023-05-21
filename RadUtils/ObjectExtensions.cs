using System.Reflection;

namespace RadUtils;

/// <summary>
///   Extension methods which apply to all objects.
/// </summary>
public static class ObjectExtensions {
  /// <summary>
  ///   Calls a private method on an object. A dangerous, but occasionally useful tool.
  /// </summary>
  /// <param name="obj"> The object to call the method on. </param>
  /// <param name="methodName">
  ///   The name of the method to call. This may only be a <c> private </c> or <c> protected </c>
  ///   member of the object.
  /// </param>
  /// <param name="args"> The arguments to pass to the method. </param>
  /// <returns> The return value of the method. </returns>
  /// <exception cref="InvalidOperationException"> Thrown if the method does not exist. </exception>
  public static object? Call(this object obj, string methodName, params object[] args) {
    var methodInfo =
      obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
    if (methodInfo == null) {
      throw new InvalidOperationException(
          $"Method \"{methodName}\" was not found on class \"{obj.GetType().Name}\"."
        );
    }

    return methodInfo?.Invoke(obj, args);
  }


  /// <inheritdoc cref="ObjectExtensions.Call" />
  /// <typeparam name="T"> The return type of the method. </typeparam>
  public static T Call<T>(this object obj, string methodName, params object[] args) {
    return (T)obj.Call(methodName, args);
  }


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
