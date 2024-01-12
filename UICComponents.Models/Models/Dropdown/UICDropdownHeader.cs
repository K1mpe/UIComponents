using UIComponents.ComponentModels.Interfaces;

namespace UIComponents.ComponentModels.Models.Dropdown;

/// <summary>
/// A title in a dropdownlist
/// </summary>
public class UICDropdownHeader : UIComponent, IDropdownItem, IHasIcon
{


    #region Ctor
    public UICDropdownHeader()
    {

    }

    public UICDropdownHeader(string content)
    {
        Content = content;
    }
    #endregion

    #region Properties

    public string Content { get; set; }
    public ITranslationModel Tooltip { get; set; }
    public UICIcon Icon { get; set; }


    #endregion
}
