using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Buttons;

/// <summary>
/// A buttongroup will combine multiple buttons and make them appear like one cohesive unit
/// </summary>
public class UICButtonGroup : UIComponent, IUICHasAttributesAndChildren, IUICSupportsTaghelperContent
{

    #region Ctor
    public UICButtonGroup()
    {

    }
    public UICButtonGroup(List<IUIComponent> buttons)
    {
        Buttons = buttons;
    }
    #endregion

    #region Properties
    public List<IUIComponent> Buttons { get; set; } = new();

    public bool VerticalButtons { get; set; }

    public List<IUIComponent> Children => Buttons;
    #endregion

    bool IUICSupportsTaghelperContent.CallWithEmptyContent => false;
    /// <inheritdoc cref="IUICSupportsTaghelperContent.SetTaghelperContent(string)"/>>
    protected virtual Task SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes)
    {
        var child = new UICCustom(taghelperContent);
        this.Add(child);
        return Task.CompletedTask;
    }
    Task IUICSupportsTaghelperContent.SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes) => SetTaghelperContent(taghelperContent, attributes);
}
