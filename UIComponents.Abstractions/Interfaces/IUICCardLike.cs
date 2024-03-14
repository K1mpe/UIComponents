namespace UIComponents.Abstractions.Interfaces;


public interface IUICCardLike : IUICTab
{
    public IHeader Header { get;}
    public IUICHasAttributesAndChildren Content { get; }
    IUICHasAttributes IUICTab.Content => Content;


    public IUICHasAttributesAndChildren Footer { get; }
}