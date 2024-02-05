using UIComponents.Abstractions.Extensions;

namespace UIComponents.Models.Models.Buttons;


/// <summary>
/// The save button triggers the <see cref="UICActionSubmit"/> and has a default appearance.
/// </summary>
public class UICButtonSave : UICButton
{
    public UICButtonSave()
    {
        ButtonText = TranslationDefaults.ButtonSave;
        Color = ColorDefaults.ButtonSave?? ColorDefaults.ButtonDefault;
        this.AddAttribute("class", "btn-save");
    }
}
