using UIComponents.Abstractions.Models;
using UIComponents.Models.Models.Actions;

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
    public UICForm(UICActionSubmit submitAction) : base()
    {
        Children.Add(submitAction);
    }
    #endregion

    #region Properties

    public List<IUIComponent> Children { get; set; } = new();

    /// <summary>
    /// On loading, set the focus on the first input field
    /// </summary>
    public bool SetFocusOnFirstInput { get; set; } = true;

    public bool Readonly { get; set; }

    #endregion


}
