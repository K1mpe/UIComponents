namespace UIComponents.Generators.Models;

internal class StoredComponentModel
{
    public IUIComponent StoredComponent { get; set; }
    public DateTime MaxLifeTime { get; set; }
    public bool SingleUse { get; set; }
}
