using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Actions;

public class UICActionOpenResultAsModal : IUICAction
{
    public string RenderLocation => UIComponent.DefaultIdentifier(nameof(UICActionOpenResultAsModal));
    #region Ctor
    public UICActionOpenResultAsModal()
    {

    }
    #endregion

    public string ResultPropertyName = "result";

    
}
