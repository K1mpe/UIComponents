using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Actions;

/// <summary>
/// Close the nearest modal or popup, if no modal or popup is available, trigger "OnFailed"
/// </summary>
public class UICActionRefreshPartial : UIComponent, IUIAction
{
    #region Ctor
    public UICActionRefreshPartial(UICPartial? partial = null, IUICHasAttributes spinElement=null) : base()
    {
        Partial = partial;
        SpinElement = spinElement;
    }
    #endregion


    #region Properties
    /// <summary>
    /// The partial that will be updated, if null, search the DOM tree for the closest partial
    /// </summary>
    [IgnoreGetChildrenFunction]
    public UICPartial? Partial { get; set; } = null;


    /// <summary>
    /// When this partial is refreshing, this element will spin. Usefull for the icon of button
    /// </summary>
    /// <remarks>
    /// If using a icon, be sure to generate a id for the icon, since a icon does not generate one itself.</remarks>
    [IgnoreGetChildrenFunction]
    public IUICHasAttributes? SpinElement { get; set; } = null;
    #endregion

}
