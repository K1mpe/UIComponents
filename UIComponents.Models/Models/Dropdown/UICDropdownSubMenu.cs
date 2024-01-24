namespace UIComponents.Models.Models.Dropdown;

public class UICDropdownSubMenu : UICDropdownItem, IUICHasChildren<IDropdownItem>
{
    #region Ctor
    public UICDropdownSubMenu()
    {

    }

    public UICDropdownSubMenu(ITranslateable content, List<IDropdownItem> items) : base(content, null)
    {
        Items = items ?? new();
    }
    #endregion

    #region Properies

    public List<IDropdownItem> Items { get; set; } = new();

    /// <summary>
    /// Render is always false if there are no subItems
    /// </summary>
    public override bool Render
    {
        get
        {

            if (!Items.Any())
                return false;
            return base.Render;
        }
        set => base.Render = value;
    }

    /// <summary>
    /// If this submenu only contains 1 item, no submenu will be rendered and only the single item will be.
    /// </summary>
    public bool ReplaceBySingleItem { get; set; } = true;

    List<IDropdownItem> IUICHasChildren<IDropdownItem>.Children => Items;

    #endregion

    #region Methods
    public UICDropdownSubMenu Add<T>(out T added, T item) where T : IDropdownItem
    {
        return this.Add<UICDropdownSubMenu, T, IDropdownItem>(out added, item);
    }

    public UICDropdownSubMenu Add(IDropdownItem item)
    {
        return this.Add<UICDropdownSubMenu, IDropdownItem>(item);
    }

    public UICDropdownSubMenu Add<T>(T item, Action<T> configure) where T : IDropdownItem
    {
        return this.Add<UICDropdownSubMenu, T, IDropdownItem>(item, configure);
    }
    #endregion
}
