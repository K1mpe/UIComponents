namespace UIComponents.Abstractions.Models;

/// <summary>
/// These are the actions available for Inputs
/// </summary>
public class UICActions : UIComponent, IUIAction
{
    #region Fields

    /// <summary>
    /// This is a list of all the actions that are assigned in the UIActions class
    /// </summary>
    public List<IUIAction> AllActions
    {
        get
        {
            return new List<IUIAction>()
            {
                OnClick,
                OnChange,
                AfterChange,
                OnFocus,
                OnLoseFocus
            };
        }
    }

    public override bool Render { get => AllActions.Where(x => x != null).Any(); set => throw new Exception("Cannot set Render attribute on UIActions"); }
    #endregion

    #region Ctor
    public UICActions() : base()
    {

    }
    #endregion

    #region Properties

    /// <summary>
    /// Action triggered when clicking on the element
    /// </summary>
    public IUIAction OnClick { get; set; } = new UICCustom();

    /// <summary>
    /// Action triggered on each change (key-up)
    /// </summary>
    public IUIAction OnChange { get; set; } = new UICCustom();

    /// <summary>
    /// Action triggered after the value has updated (focus out & change)
    /// </summary>
    public IUIAction AfterChange { get; set; } = new UICCustom();

    /// <summary>
    /// Action triggered when element gets focus
    /// </summary>
    public IUIAction OnFocus { get; set; } = new UICCustom();

    /// <summary>
    /// Action triggered when element loses focus
    /// </summary>
    public IUIAction OnLoseFocus { get; set; } = new UICCustom();
    #endregion
}
