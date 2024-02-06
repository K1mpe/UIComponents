namespace UIComponents.Models.Models.Dropdown;

public class UICDropdown : UIComponent, IUICHasChildren<IDropdownItem>
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

    public UICDropdown(Translatable dropdownText, List<IDropdownItem> dropdownItems = null)
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

    List<IDropdownItem> IUICHasChildren<IDropdownItem>.Children => DropdownItems;

    #region Methods
    /// <summary>
    /// Add a item to the collection and return the <paramref name="item"/>
    /// </summary>
    /// <returns><paramref name="item"/></returns>
    public UICDropdown Add<T>(out T added,T item) where T : IDropdownItem
    {
        return this.Add<UICDropdown,T, IDropdownItem>(out added,item);
    }

    public UICDropdown Add(IDropdownItem item)
    {
        return this.Add<UICDropdown, IDropdownItem>(item);
    }

    public UICDropdown Add<T>(T item, Action<T> configure) where T : IDropdownItem
    {
        return this.Add<UICDropdown, T, IDropdownItem>(item, configure);
    }

    #endregion

    #region Converters
    public UICDropdownSubMenu ConvertToSubMenu()
    {
        var subMenu = CommonHelper.Convert<UICDropdownSubMenu>(this);
        if(Button is UICButton button)
        {
            subMenu.Content = button.ButtonText;
            subMenu.Icon = button.PrependButtonIcon;
            subMenu.Items = DropdownItems;
        }
        subMenu.ReplaceBySingleItem = ReplaceDropdownByButtonIfSingleDropdownItem;
        return subMenu;
    }
    #endregion

    public enum IconPositionEnum
    {
        Left,
        Right,
        None,
    }
}
