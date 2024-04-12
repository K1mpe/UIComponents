
namespace UIComponents.Models.Models.Actions;

public class UICActionRefresh : UICCustom
{
    /// <summary>
    /// Trigger reload on this element
    /// </summary>
    public UICActionRefresh()
    {
    }


    [UICIgnoreGetChildrenFunction]
    public IUICHasAttributes Target { get; set; } = null;

    public override Task InitializeAsync()
    {
        string selector = "this";

        if (Target != null)
            selector = Target.GetId();

        Content = $"$({selector}).trigger('uic-reload');";
        return base.InitializeAsync();
    }
}
