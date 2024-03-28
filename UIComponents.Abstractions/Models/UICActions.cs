namespace UIComponents.Abstractions.Models;

/// <summary>
/// These are the actions available for Inputs
/// </summary>
public class UICActions : UIComponent, IUICAction
{
    #region Fields

    public override string RenderLocation => this.CreateDefaultIdentifier(Renderer);
    /// <summary>
    /// This is a list of all the actions that are assigned in the UIActions class
    /// </summary>
    public List<IUICAction> AllActions
    {
        get
        {
            return new List<IUICAction>()
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
    public UICActions(Func<string> id) : base()
    {
        Id = id;
    }
    #endregion

    #region Properties

    public Func<string> Id { get; set; }

    /// <summary>
    /// Action triggered when clicking on the element
    /// </summary>
    public IUICAction OnClick { get; set; } = new UICCustom();

    /// <summary>
    /// Action triggered on each change (key-up)
    /// </summary>
    public IUICAction OnChange { get; set; } = new UICCustom();

    /// <summary>
    /// Action triggered after the value has updated (focus out & change)
    /// </summary>
    public IUICAction AfterChange { get; set; } = new UICCustom();

    /// <summary>
    /// Action triggered when element gets focus
    /// </summary>
    public IUICAction OnFocus { get; set; } = new UICCustom();

    /// <summary>
    /// Action triggered when element loses focus
    /// </summary>
    public IUICAction OnLoseFocus { get; set; } = new UICCustom();


    /// <summary>
    /// Automatically toggled for select2 parents
    /// </summary>
    public ActionsRenderer Renderer { get; set; } = ActionsRenderer.Default;
    #endregion
    public enum ActionsRenderer
    {
        Default,
        Select2,
    }
}

