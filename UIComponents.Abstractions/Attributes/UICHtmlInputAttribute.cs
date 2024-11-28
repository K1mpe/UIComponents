namespace UIComponents.Abstractions.Attributes;

/// <summary>
/// When rendering a <see cref="IUIComponent"/> from this property with the generator, this attribute is placed on the input
/// <b>Multiple attributes are possible</b>
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public class UICHtmlInputAttribute : Attribute
{
    public UICHtmlInputAttribute(string attributeName, object attributeValue)
    {
        AttributeName = attributeName;
        AttributeValue = attributeValue;
    }

    public string AttributeName { get; set; }
    public object AttributeValue { get; set; }
}