using UIComponents.Abstractions.Varia;

namespace UIComponents.Abstractions.Interfaces;

public interface IUIComponent
{
    /// <summary>
    /// The Unique identifier can be used to use diffrent renders for the same object.
    /// <br>If the last 7 characters do not contain a '.'  , '.cshtml' is automatically added</br>
    /// </summary>
    /// <remarks>
    /// Example: /Views/Shared/Components/Button/Default
    /// </remarks>
    public string RenderLocation { get; }
}

