namespace UIComponents.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class UICTooltipAttribute : Attribute
{
    /// <summary>
    /// When rendering a UIC component, this property will popup with this text when hovering over the element
    /// </summary>
    /// <param name="translationKey"></param>
    /// <param name="defaultText"></param>
    public UICTooltipAttribute(string defaultText, string translationKey=null)
    {
        TranslationModel = new Translatable(translationKey, defaultText);
    }

    public Translatable TranslationModel { get; init; }
}
