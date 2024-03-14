namespace UIComponents.Abstractions.Interfaces;

public interface IUICTab : IUIComponent
{
    public IHeader Header { get; }
    public IUICHasAttributes Content { get;}
}
