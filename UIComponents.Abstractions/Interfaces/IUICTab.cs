namespace UIComponents.Abstractions.Interfaces;

public interface IUICTab : IUIComponent, IUICHasColor
{
    public IHeader Header { get; }
    public IUICHasAttributes Content { get;}

    IColor IUICHasColor.Color => Header?.Color ?? null;
}
