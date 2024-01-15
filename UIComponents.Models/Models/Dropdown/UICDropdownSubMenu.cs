namespace UIComponents.Models.Models.Dropdown;

public class UICDropdownSubMenu : UICDropdownItem
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

    #endregion

    #region Methods
    /// <summary>
    /// Add a item to the collection and return the <paramref name="item"/>
    /// </summary>
    /// <returns><paramref name="item"/></returns>
    public T Add<T>(T item) where T : class, IDropdownItem
    {
        Items.Add(item);
        return item;
    }

    /// <summary>
    /// Add a item to the collection and return the current <see cref="UICDropdownSubMenu"/>
    /// </summary>
    /// <returns><see cref="UICDropdownSubMenu"/></returns>
    public UICDropdownSubMenu Add2(IDropdownItem item)
    {
        Items.Add(item);
        return this;
    }
    #endregion
}
