namespace UIComponents.Abstractions.Attributes;

/// <summary>
/// When rendering a UIC component, this property will popup with this text when hovering over the element
/// <br></br>To change the default icon, use the <see cref="UICTooltipIconAttribute"/> attribute
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class UICTooltipAttribute : Attribute
{
    /// <see cref="UICTooltipAttribute"/>
    /// <param name="translationKey">This replaces the default translationKey</param>
    /// <param name="defaultText">The text in the default language</param>
    public UICTooltipAttribute(string defaultText, string translationKey = null)
    {
        TranslationModel = new Translatable(translationKey, defaultText);
    }

    public Translatable TranslationModel { get; init; }
}
