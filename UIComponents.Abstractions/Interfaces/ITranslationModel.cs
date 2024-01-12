namespace UIComponents.Abstractions.Interfaces;

public interface ITranslationModel
{
    public string ResourceKey { get; set; }
    public string DefaultValue { get; set; }
    public object[] Arguments { get; set; }


}
