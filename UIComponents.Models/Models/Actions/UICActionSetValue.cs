using UIComponents.Models.Abstract;

namespace UIComponents.Models.Models.Actions;

/// <summary>
/// This action will set a value clientside
/// </summary>
public class UICActionSetValue : UIComponent, IUIAction
{

    #region Ctor
    public UICActionSetValue()
    {

    }

    public UICActionSetValue(IUIComponent component, string valueName = "referenceObject")
    {
        Component = component;
        ValueName = valueName;
    }
    #endregion

    #region Properties

    /// <summary>
    /// The component to set the value too
    /// </summary>
    [IgnoreGetChildrenFunction]
    public IUIComponent Component { get; set; }

    /// <summary>
    /// This is the name of the variable used to set the value
    /// <br>Example: the name of the value received from signalR</br>
    /// </summary>
    public string ValueName { get; set; } = "referenceObject";
    #endregion
}
