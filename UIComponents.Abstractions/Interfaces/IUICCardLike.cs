namespace UIComponents.Abstractions.Interfaces;


public interface IUICCardLike : IUICTab, IUICHasAttributesAndChildren
{
    public IUICHeader Header { get; }
    public IUICHasAttributesAndChildren Content { get; }
    IUICHasAttributes IUICTab.Content => Content;

    List<IUIComponent> IUICHasChildren<IUIComponent>.Children => Content.Children;

    public IUICHasAttributesAndChildren Footer { get; }
}