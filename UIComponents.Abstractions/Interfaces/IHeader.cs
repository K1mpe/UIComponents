namespace UIComponents.Abstractions.Interfaces;

public interface IHeader : IUIComponent
{
    public ITranslateable Title { get; set; }
    public IColor? Color { get; set; }
}
