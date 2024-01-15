namespace UIComponents.Abstractions.Interfaces;

public interface IUITabCard : IUIComponent
{
    public ITranslateable Title { get; set; }

    public IColor? HeaderColor { get; }
}
