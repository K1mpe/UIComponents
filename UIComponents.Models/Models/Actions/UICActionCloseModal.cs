using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Actions;


/// <summary>
/// Close the nearest modal or popup, if no modal or popup is available, trigger "OnFailed"
/// </summary>
public class UICActionCloseModal : UIComponent, IUIAction
{
    #region Fields
    #endregion

    #region Ctor
    public UICActionCloseModal() : base()
    {

    }
    #endregion

    #region Properties
    /// <summary>
    /// This function is called where there is no card available to close
    /// </summary>
    public IUIAction OnFailed { get; set; } = new UICCustom();
    #endregion
}
