namespace UIComponents.Abstractions.Interfaces;

public interface IUIComponent
{
    /// <summary>
    /// The Unique identifier can be used to use diffrent renders for the same object.
    /// </summary>
    /// <remarks>
    /// Example: /Views/Shared/Components/UIComponent/Button/_Default
    /// </remarks>
    public string RenderLocation { get; }

}
