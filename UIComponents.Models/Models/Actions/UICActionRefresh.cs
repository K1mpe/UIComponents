
namespace UIComponents.Models.Models.Actions;

public class UICActionRefresh : UICCustom
{
    /// <summary>
    /// Trigger reload on this element
    /// </summary>
    public UICActionRefresh(IUICHasAttributes target = null)
    {
        Target = target;
        Content = "$(this).trigger('uic-reload');";
    }


    [UICIgnoreGetChildrenFunction]
    public IUICHasAttributes Target { get; set; } = null;

    protected override Task InitializeAsync()
    {
        string selector = "this";

        if (Target != null)
            selector = $"'#{Target.GetId()}'";
        else
        {
            string identifier = $"'{this.GetAttribute("identifier")}'";
            selector = identifier??selector;
        }

        Content = $"$({selector}).trigger('uic-reload');";
        return base.InitializeAsync();
    }
}
