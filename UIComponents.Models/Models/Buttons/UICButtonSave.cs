namespace UIComponents.Models.Models.Buttons;


/// <summary>
/// The save button triggers the <see cref="UICActionSubmit"/> and has a default appearance.
/// </summary>
public class UICButtonSave : UICButton
{
    public UICButtonSave()
    {
        ButtonText = TranslationDefaults.ButtonSave;
        PrependButtonIcon = IconDefaults.Save?.Invoke();
        Color = ColorDefaults.ButtonSubmit?.Invoke() ?? ColorDefaults.ButtonDefault?.Invoke();
        this.AddAttribute("class", "btn-save");
    }
}

