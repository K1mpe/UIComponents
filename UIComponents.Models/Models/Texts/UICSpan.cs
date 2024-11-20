using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Texts;

/// <summary>
/// A normal span text
/// </summary>
public class UICSpan : UIComponent, IUICSupportsTaghelperContent
{

    #region Ctor
    public UICSpan() : base()
    {

    }
    public UICSpan(Translatable text) : this()
    {
        Text = text;
    }
    #endregion
    public Translatable Text { get; set; }


    bool IUICSupportsTaghelperContent.CallWithEmptyContent => false;

    /// <inheritdoc cref="IUICSupportsTaghelperContent.SetTaghelperContent(string)"/>>
    protected virtual Task SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes)
    {
        Text = taghelperContent;
        return Task.CompletedTask;
    }
    Task IUICSupportsTaghelperContent.SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes) => SetTaghelperContent(taghelperContent, attributes);
}
