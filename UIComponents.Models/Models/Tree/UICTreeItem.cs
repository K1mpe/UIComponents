using UIComponents.Abstractions.Interfaces;

namespace UIComponents.Models.Models.Tree;

public class UICTreeItem : IUIComponent, IUICHasChildren<UICTreeItem>
{
    #region Fields

    public string RenderLocation => this.CreateDefaultIdentifier();

    #endregion

    #region Ctor
    public UICTreeItem()
    {

    }
    public UICTreeItem(Translatable text)
    {
        Text= text;
    }
    #endregion

    #region Properties
    public string Id { get; set; }

    public Translatable Text { get; set; }

    public Translatable Tooltip { get; set; }
    public string Icon { get; set; }

    public JsTreeItemState State { get; set; } = new();

    public List<UICTreeItem> Children { get; set; } = new();

    /// <summary>
    /// If no <see cref="Children"/> are provided, assume this item has child elements through the ajax call
    /// </summary>
    /// <remarks>
    /// Ajax data is configured in <see cref="JsTree"/>
    /// </remarks>
    public bool HasAjaxChildren { get; set; } = true;

    public Dictionary<string, string> Li_Attr { get; set; } = new();
    public Dictionary<string, string> A_Attr { get; set; } = new();

    #endregion

    public UICTreeItem Add(UICTreeItem item)
    {
        return this.Add<UICTreeItem, UICTreeItem>(item);
    }

    public UICTreeItem Add(out UICTreeItem added, UICTreeItem item)
    {
        return this.Add<UICTreeItem, UICTreeItem, UICTreeItem>(out added, item);
    }

    public UICTreeItem Add(UICTreeItem item, Action<UICTreeItem> configure)
    {
        return this.Add(item, configure);
    }
    public class JsTreeItemState
    {
        public bool Opened { get; set; }
        public bool Disabled { get; set; }
        public bool Selected { get; set; }

    }
}

