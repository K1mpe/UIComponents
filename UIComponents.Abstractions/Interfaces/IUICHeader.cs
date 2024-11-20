namespace UIComponents.Abstractions.Interfaces;

public interface IUICHeader : IUIComponent, IUICHasAttributes
{
    public Translatable Title { get; set; }
    public IColor? Color { get; set; }

    /// <summary>
    /// When a header is used, it runs the Transformer first.
    /// <br>This may give you the chance to alter the header to better suit its usecase</br>
    /// </summary>
    /// <remarks>
    /// possible types of the input object:
    /// <br>UICCard</br>
    /// <br>UICTabs</br>
    /// </remarks>
    public Func<object, IUICHeader,Task> Transformer { get; set; }
}
