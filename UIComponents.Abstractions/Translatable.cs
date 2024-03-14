using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

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
        {
            if (Arguments.Any())
                return string.Format(DefaultValue, Arguments);
            else return DefaultValue;
        }
            

        if (ResourceKey.Contains(".") && !ResourceKey.Trim().EndsWith("."))
            return ResourceKey.Split('.').Last();

        return ResourceKey;
    }

    public virtual string Serialize()
    {
        string serialized = JsonSerializer.Serialize(this);
        return $"[TRANSLATABLE]{serialized}";
    }
    public static implicit operator Translatable(string text){
        if(!string.IsNullOrEmpty(text) && text.StartsWith("[TRANSLATABLE]"))
        {
            string serialised = text.Substring(14);
            var deserialised =  JsonSerializer.Deserialize<Translatable>(serialised);
            for(int i= 0; i < deserialised.Arguments.Length; i++)
            {
                var argument = deserialised.Arguments[i];
                if(argument is JsonElement jsonElement && argument.ToString().Contains(nameof(ResourceKey)))
                {
                    deserialised.Arguments[i] = (Translatable)argument.ToString();
                }
            }
            return deserialised;
        }
        return new Untranslated(text);
    }

}

/// <summary>
/// This is a override on Translatable that will not create a new resourceKey
/// </summary>
public class Untranslated : Translatable
{
    public Untranslated(string text) : base("UntranslatedKey", "{0}", text)
    {
    }

    public override string Serialize()
    {
        return Arguments[0]?.ToString();
    }
}

