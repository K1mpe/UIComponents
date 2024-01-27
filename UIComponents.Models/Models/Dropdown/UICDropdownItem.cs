using UIComponents.Abstractions.Models;
using UIComponents.Models.Models.Icons;

namespace UIComponents.Models.Models.Dropdown;

/// <summary>
/// A item inside the dropdown
/// </summary>
public class UICDropdownItem : UIComponent, IDropdownItem, IHasIcon<UICIcon>
{


    #region Ctor
    public UICDropdownItem()
    {

    }

    public UICDropdownItem(Translatable content, IUIAction onClick = null)
    {
        Content = content;
        if (onClick != null)
            OnClick = onClick;
    }
    #endregion


    #region Properties
    public Translatable Content { get; set; }

    /// <summary>
    /// When hovering over the DropdownItem, this text will be shown
    /// </summary>
    public Translatable Tooltip { get; set; }

    /// <summary>
    /// Clicking a dropdown element
    /// </summary>
    /// <remarks>
    /// Available args: e => eventArgs
    /// </remarks>
    public IUIAction OnClick { get; set; } = new UICCustom();
    public UICIcon Icon { get; set; }




    #endregion

}
