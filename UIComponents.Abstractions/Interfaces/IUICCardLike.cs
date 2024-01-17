namespace UIComponents.Abstractions.Interfaces;


public interface IUICCardLike : IUICTab
{
    public IHeader Header { get; set; }
    public IUIComponent Content { get; set; }
    
    public IUIComponent? Footer { get; set; }
}