using UIComponents.Abstractions.Extensions;

namespace UIComponents.Models.Models.Actions;

public class UICActionNYI : IUICAction
{
    public string RenderLocation => this.CreateDefaultIdentifier();

    public bool Render => true;
}
