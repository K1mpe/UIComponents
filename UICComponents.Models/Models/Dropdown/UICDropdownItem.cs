using UIComponents.ComponentModels.Interfaces;

namespace UIComponents.ComponentModels.Models.Dropdown;

/// <summary>
/// A item inside the dropdown
/// </summary>
public class UICDropdownItem : UIComponent, IDropdownItem, IHasIcon
{


    #region Ctor
    public UICDropdownItem()
    {

    }

    public UICDropdownItem(ITranslationModel content, IUIAction onClick = null)
    {
        Content = content;
        if (onClick != null)
            OnClick = onClick;
    }
    #endregion


    #region Properties
    public ITranslationModel Content { get; set; }

    /// <summary>
    /// When hovering over the DropdownItem, this text will be shown
    /// </summary>
    public ITranslationModel Tooltip { get; set; }

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
