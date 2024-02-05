namespace UIComponents.Abstractions;

public class Translatable
{
    public Translatable() { }
    public Translatable(string resourceKey, string defaultValue = null, params object[] args)
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

        if (ResourceKey.Contains(".") && !ResourceKey.Trim().EndsWith("."))
            return ResourceKey.Split('.').Last();

        return ResourceKey;
    }

    public static implicit operator Translatable(string text) => new Untranslated(text);

}

/// <summary>
/// This is a override on Translatable that will not create a new resourceKey
/// </summary>
public class Untranslated : Translatable
{
    public Untranslated(string text) : base("UntranslatedKey", "{0}", text)
    {
    }
}

