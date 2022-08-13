namespace RadParser.Utils.Attributes;

/// <summary>
/// An attribute that defines that a property's value is computed.
/// This is used to indicate that a node's property shouldn't be
/// used until the entire AST is built and normalized.
/// </summary>
[AttributeUsage(AttributeTargets.Property)
]
public class ComputedAttribute : Attribute {
}
