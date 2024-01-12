using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Actions;

/// <summary>
/// This action will set a form as readonly
/// </summary>
public class UICActionSetReadonly : UIComponent, IUIAction
{

    public UICActionSetReadonly() : base()
    {

    }

    #region Properties
    public bool ShowEmptyInReadonly { get; set; }
    public bool ShowSpanInReadonly { get; set; }

    public bool ShowDeleteButtonInReadonly { get; set; }
    #endregion
}
