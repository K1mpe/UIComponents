using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Actions;

/// <summary>
/// Close the nearest modal or popup, if no modal or popup is available, trigger "OnFailed"
/// </summary>
public class UICActionRefreshPartial : UIComponent, IUIAction
{
    #region Ctor
    public UICActionRefreshPartial(UICPartial? partial = null) : base()
    {

    }
    #endregion


    #region Properties
    /// <summary>
    /// The partial that will be updated, if null, search the DOM tree for the closest partial
    /// </summary>
    public UICPartial? Partial { get; set; } = null;
    #endregion

}
