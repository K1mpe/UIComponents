using UIComponents.Models.Extensions;

namespace UIComponents.Models.Models.Actions;


/// <summary>
/// Navigate to a certain page.
/// </summary>
public class UICActionNavigate : IUIAction
{
    public UICActionNavigate()
    {
    }

    public UICActionNavigate(string href) : this()
    {
        Href = href;
    }

    public string Href { get; set; }



    public string RenderLocation => this.CreateDefaultIdentifier();

    public bool Render => true;
}
