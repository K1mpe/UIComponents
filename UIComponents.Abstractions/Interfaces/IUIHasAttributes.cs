namespace UIComponents.Abstractions.Interfaces;

public interface IUIHasAttributes : IUIComponent
{
    Dictionary<string, string> Attributes { get; set; }
}
