namespace UIComponents.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property)]
/// <summary>
/// Applying this attribute to a property will ignore this property for the GetAllChildren() extention function
/// </summary>
public class UICIgnoreGetChildrenFunctionAttribute : Attribute
{
}
