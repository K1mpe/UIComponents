using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Actions;


/// <summary>
/// Close the nearest modal or popup, if no modal or popup is available, trigger "OnFailed"
/// </summary>
public class UICActionCloseModal : UIComponent, IUICAction
{
    #region Fields
    #endregion

    #region Ctor
    public UICActionCloseModal(string selector = null) : base()
    {
        Selector = selector;
    }
    #endregion

    #region Properties

    public string Selector { get; set; }

    /// <summary>
    /// This function is called where there is no modal available to close
    /// </summary>
    public IUICAction OnFailed { get; set; } = new UICCustom();
    #endregion
}
