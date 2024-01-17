namespace UIComponents.Abstractions.Interfaces;

public interface IUICTab
{
    public IHeader Header { get; set; }
    public IUIHasAttributes Content { get; set; }
}
