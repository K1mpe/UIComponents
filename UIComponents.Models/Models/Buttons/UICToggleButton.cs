using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Buttons;

[PrependAppendInputGroupClass("input-group-button")]
/// <summary>
/// This has 2 buttons, and will change between them if the <see cref="Value"/> is changed.
/// </summary>
public class UICToggleButton : UIComponent
{
    #region Ctor
    public UICToggleButton()
    {

    }

    #endregion

    public bool Value { get; set; }

    /// <summary>
    /// When true, you will not automatically change when clicking the button
    /// </summary>
    public bool DisableAutoChange { get; set; }

    public UICButton ButtonTrue { get; set; }
    public UICButton ButtonFalse { get; set; }
}
