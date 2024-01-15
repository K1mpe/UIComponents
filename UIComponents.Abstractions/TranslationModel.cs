namespace UIComponents.Abstractions;

public partial class TranslationModel : ITranslateable
{
    public TranslationModel() { }
    public TranslationModel(string resourceKey, string defaultValue = null, params object[] args)
    {
        ResourceKey = resourceKey;
        DefaultValue = defaultValue;
        Arguments = args;
    }


    public string ResourceKey { get; set; }
    public string DefaultValue { get; set; }
    public object[] Arguments { get; set; }

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(DefaultValue))
            return string.Format(DefaultValue, Arguments);

        return ResourceKey;
    }

    public static implicit operator TranslationModel(string text) => new Untranslated(text);

}

/// <summary>
/// This is a override on ITranslateable that will not create a new resourceKey
/// </summary>
public class Untranslated : TranslationModel
{
    public Untranslated(string text) : base("UntranslatedKey", "{0}", text)
    {
    }
}

