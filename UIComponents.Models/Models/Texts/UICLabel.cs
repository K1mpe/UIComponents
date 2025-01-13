
namespace UIComponents.Models.Models.Texts;

/// <summary>
/// A label, used in a <see cref="UICInputGroup"/>
/// </summary>
public class UICLabel : UIComponent , IUICSupportsTaghelperContent
{
    #region Ctor
    public UICLabel() : base()
    {

    }
    public UICLabel(Translatable labelText) : this()
    {
        LabelText = labelText;
    }
    #endregion


    public Translatable LabelText { get; set; }
    public Translatable Tooltip { get; set; }

    public List<IUIComponent> PrependLabel { get; set; } = new();
    public List<IUIComponent> AppendLabel { get; set; } = new();


    /// <summary>
    /// This icon is displayed only when the tooltip is not empty
    /// </summary>
    public UICIcon TooltipIcon { get; set; } = IconDefaults.TooltipIcon?.Invoke()?.AddClass("tooltip-icon");

    /// <summary>
    /// Adding a required marker behind this label
    /// </summary>
    public bool Required { get; set; }

    bool IUICSupportsTaghelperContent.CallWithEmptyContent => false;

    /// <inheritdoc cref="IUICSupportsTaghelperContent.SetTaghelperContent(string)"/>>
    protected virtual Task SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes)
    {
        LabelText = taghelperContent;
        return Task.CompletedTask;
    }
    Task IUICSupportsTaghelperContent.SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes) => SetTaghelperContent(taghelperContent, attributes);
}
