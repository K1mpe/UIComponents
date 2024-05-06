namespace UIComponents.Models.Models.Actions;


/// <summary>
/// This method uses the uic.markChanges() method.
/// <br>This will change readonly values & notify the user of changed input fields.</br>
/// </summary>
public class UICActionMarkChanges : IUICAction
{
    #region Fields
    public string RenderLocation => this.CreateDefaultIdentifier();

    #endregion

    #region Ctor
    public UICActionMarkChanges()
    {

    }

    /// <summary>
    /// Calls the uic.markChanges($(selector), valueName) function clientside
    /// </summary>
    /// <param name="selector">Function that leads to a id or class. can also be a <see cref="UIComponent"/></param>
    /// <param name="valueName"></param>
    public UICActionMarkChanges(Func<string> selector, string valueName = "referenceObject")
    {
        Selector = selector;
        ValueName = valueName;
    }
    #endregion

    #region Properties

    /// <summary>
    /// The selector
    /// </summary>
    public Func<string> Selector { get; set; }

    /// <summary>
    /// This is the name of the variable used to set the value
    /// <br>Example: the name of the value received from signalR</br>
    /// </summary>
    public string ValueName { get; set; } = "referenceObject";
    #endregion
}
