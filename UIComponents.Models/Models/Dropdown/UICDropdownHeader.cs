using UIComponents.Abstractions.Models;
using UIComponents.Models.Models.Icons;

namespace UIComponents.Models.Models.Dropdown;

/// <summary>
/// A title in a dropdownlist
/// </summary>
public class UICDropdownHeader : UIComponent, IDropdownItem, IHasIcon<UICIcon>
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
    public Translatable Tooltip { get; set; }
    public UICIcon Icon { get; set; }



    #endregion
}
