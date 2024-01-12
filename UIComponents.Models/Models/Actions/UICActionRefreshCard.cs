using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Actions;

/// <summary>
/// Close the nearest modal or popup, if no modal or popup is available, trigger "OnFailed"
/// </summary>
public class UICActionRefreshCard : UIComponent, IUIAction
{
    #region Ctor
    public UICActionRefreshCard() : base()
    {

    }
    #endregion

}
