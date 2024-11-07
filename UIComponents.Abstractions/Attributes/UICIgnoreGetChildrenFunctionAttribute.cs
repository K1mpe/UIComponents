namespace UIComponents.Abstractions.Attributes;
/// <summary>
/// Applying this attribute to a property will ignore this property for the GetAllChildren() extention function
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public class UICIgnoreGetChildrenFunctionAttribute : Attribute
{
}
