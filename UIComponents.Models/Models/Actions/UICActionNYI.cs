using UIComponents.Abstractions.Extensions;

namespace UIComponents.Models.Models.Actions;

public class UICActionNYI : IUIAction
{
    public string RenderLocation => this.CreateDefaultIdentifier();

    public bool Render => true;
}
