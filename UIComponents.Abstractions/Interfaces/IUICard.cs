namespace UIComponents.Abstractions.Interfaces;

public interface IUITabCard : IUIComponent
{
    public ITranslationModel Title { get; set; }

    public IColor? HeaderColor { get; }
}
