using UIComponents.Models.Defaults;
using UIComponents.Models.Extensions;
using UIComponents.Models.Models.Icons;

namespace UIComponents.Models.Models.Buttons;

public class UICButtonDelete : UICButton
{
    public UICButtonDelete(string controller, object id)
    {
        ButtonText = TranslationDefaults.ButtonDelete;
        Color = Colors.ButtonDelete;
        OnClick = new UICCustom()
        {
            Content = $"await Crud.Delete('{controller}', {id});"
        };
        PrependButtonIcon = UICIcon.Delete();
        this.AddAttribute("class", "btn-delete");
    }
}
