namespace UIComponents.Models.Models.Actions;


/// <summary>
/// This method uses the uic.markChanges() method.
/// <br>This will change readonly values & notify the user of changed input fields.</br>
/// </summary>
public class UICActionMarkChanges : IUIAction
{
    #region Fields
    public string RenderLocation => throw new NotImplementedException();

    #endregion

    #region Ctor
    public UICActionMarkChanges()
    {

    }

    public UICActionMarkChanges(IUIComponent component, string valueName = "referenceObject")
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
