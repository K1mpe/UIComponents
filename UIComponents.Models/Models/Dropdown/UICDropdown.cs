using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Dropdown;

public class UICDropdown : UIComponent
{
    #region Ctor
    public UICDropdown()
    {

    }

    public UICDropdown(IUIComponent button, List<IDropdownItem> dropdownItems = null)
    {
        Button = button;
        DropdownItems = dropdownItems ?? new();
    }

    public UICDropdown(ITranslateable dropdownText, List<IDropdownItem> dropdownItems = null)
    {
        Button = new UICButton(dropdownText);
        DropdownItems = dropdownItems ?? new();
    }

    #endregion

    /// <summary>
    /// The button that is used for this dropdown.
    /// </summary>
    /// <remarks>
    /// Other components like icons can also be used.
    /// </remarks>
    public IUIComponent Button { get; set; }

    /// <summary>
    /// These are the items that will be displayed in the dropdownMenu
    /// </summary>
    public List<IDropdownItem> DropdownItems { get; set; } = new();

    /// <summary>
    /// If none of the dropdownItems has a icon, this option is ignored.
    /// </summary>
    public IconPositionEnum IconPosition { get; set; } = IconPositionEnum.Left;

    public bool ReplaceDropdownByButtonIfSingleDropdownItem { get; set; } = true;

    #region Methods
    /// <summary>
    /// Add a item to the collection and return the <paramref name="item"/>
    /// </summary>
    /// <returns><paramref name="item"/></returns>
    public T Add<T>(T item) where T : class, IDropdownItem
    {
        DropdownItems.Add(item);
        return item;
    }

    /// <summary>
    /// Add a item to the collection and return the current <see cref="UICDropdown"/>
    /// </summary>
    /// <returns><see cref="UICDropdown"/></returns>
    public UICDropdown Add2(IDropdownItem item)
    {
        DropdownItems.Add(item);
        return this;
    }
    #endregion


    public enum IconPositionEnum
    {
        Left,
        Right,
        None,
    }
}
