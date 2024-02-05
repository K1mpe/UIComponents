namespace UIComponents.Models.Models;

/// <summary>
/// A form that can be posted. This is required to use the <see cref="UICActionSubmit"/>
/// </summary>
public class UICForm : UIComponent, IUICHasChildren<IUIComponent>
{

    #region Ctor
    public UICForm() : base()
    {

    }
    public UICForm(ISubmitAction submitAction) : base()
    {
        Submit = submitAction;
    }
    #endregion

    #region Properties

    public List<IUIComponent> Children { get; set; } = new();

    /// <summary>
    /// On loading, set the focus on the first input field
    /// </summary>
    public bool SetFocusOnFirstInput { get; set; } = true;

    /// <summary>
    /// Set the form as readonly. This will disable the submit buttons
    /// </summary>
    public bool Readonly { get; set; }


    public ISubmitAction Submit { get; set; }

    /// <summary>
    /// ClientSide: Triggers the form to post
    /// </summary>
    /// <returns></returns>
    public IUIAction? TriggerSubmit() => Readonly?null : new UICCustom($"$('#{this.GetOrGenerateId()}').trigger('submit');");

    /// <summary>
    /// ClientSide: Triggers the form to return the current value of all properties
    /// </summary>
    /// <returns></returns>
    public IUIAction TriggerGetValue() => new UICActionGetValue(this);
    #endregion

    

    //public UICForm AddChild<T>(T child, Action<T> action) where T: IUIComponent
    //{
    //    return this;
    //}

}
