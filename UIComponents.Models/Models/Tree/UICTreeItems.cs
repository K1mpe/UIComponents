namespace UIComponents.Models.Models.Tree;

public class UICTreeItems : IUIComponent, IUICHasChildren<UICTreeItem>
{
    public string RenderLocation => this.CreateDefaultIdentifier();

    public List<UICTreeItem> Items { get; set; } = new();

    public List<UICTreeItem> Children => Items;

    public UICTreeItems Add(UICTreeItem item)
    {
        return this.Add<UICTreeItems, UICTreeItem>(item);
    }
    public UICTreeItems Add(out UICTreeItem added, UICTreeItem item)
    {
        return this.Add<UICTreeItems, UICTreeItem, UICTreeItem>(out added, item);
    }

    public UICTreeItems Add(UICTreeItem item, Action<UICTreeItem> configure)
    {
        return this.Add<UICTreeItems, UICTreeItem, UICTreeItem>(item, configure);
    }
}
