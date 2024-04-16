namespace UIComponents.Abstractions.Interfaces;

/// <summary>
/// A IUIComponent that has a specific viewmodel that is used.
/// </summary>
public interface IUIComponentViewModel: IUIComponent
{
    /// <summary>
    /// This is the viewmodel that is used on the cshtml page
    /// </summary>
    public object ViewModel { get;}
}

