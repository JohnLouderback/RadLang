namespace RadParser.Utils.Attributes;

/// <summary>
/// An attribute that defines that a property's value returns one
/// or more Node that are higher in the hierarchy than the current Node.
/// Most code will assume that, otherwise, nested nodes are descendents.
/// </summary>
[AttributeUsage(AttributeTargets.Property)
]
public class AncestoralAttribute : Attribute {
}
