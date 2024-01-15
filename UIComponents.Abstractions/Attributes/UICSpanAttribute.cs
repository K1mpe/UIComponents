namespace UIComponents.Abstractions.Attributes;

public class UICSpanAttribute : Attribute
{
	/// <summary>
	/// When rendering a UIC component, this property will generate a span with this text
	/// </summary>
	/// <param name="translationKey"></param>
	/// <param name="defaultText"></param>
	public UICSpanAttribute(string translationKey, string defaultText)
	{
		TranslationModel = new TranslationModel(defaultText, translationKey);
	}

	public ITranslateable TranslationModel { get; init; }
}
