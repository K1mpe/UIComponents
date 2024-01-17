namespace UIComponents.Abstractions.Interfaces;

public interface ITranslateable
{
    public string ResourceKey { get; set; }
    public string DefaultValue { get; set; }
    public object[] Arguments { get; set; }



}
