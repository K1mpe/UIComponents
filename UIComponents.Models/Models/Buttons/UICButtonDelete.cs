using Microsoft.AspNetCore.Mvc;

namespace UIComponents.Models.Models.Buttons;

public class UICButtonDelete : UICButton
{
    public UICButtonDelete()
    {
        ButtonText = TranslationDefaults.ButtonDelete;
        Color = ColorDefaults.ButtonDelete;
        PrependButtonIcon = new UICIcon(UIComponents.Defaults.IconDefaults.Delete?.Icon ?? string.Empty);
        this.AddAttribute("class", "btn-delete");
    }

    public UICButtonDelete(Type type, object id) : this($"/{type.Name}/Delete", id)
    {
    }
    public UICButtonDelete(string url, object id) : this()
    {
        OnClick = new UICCustom($"await uic.form.delete('{url}', {id});");
    }

    
}
