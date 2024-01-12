namespace UIComponents.ComponentModels.Models.Buttons;

/// <summary>
/// A buttongroup will combine multiple buttons and make them appear like one cohesive unit
/// </summary>
public class UICButtonGroup : UIComponent
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
    #endregion
}
