namespace UIComponents.Abstractions.Attributes;

public class UICTooltipAttribute : Attribute
{
    /// <summary>
    /// When rendering a UIC component, this property will popup with this text when hovering over the element
    /// </summary>
    /// <param name="translationKey"></param>
    /// <param name="defaultText"></param>
    public UICTooltipAttribute(string translationKey, string defaultText)
    {
        TranslationModel = new TranslationModel(defaultText, translationKey);
    }

    public ITranslateable TranslationModel { get; init; }
}
