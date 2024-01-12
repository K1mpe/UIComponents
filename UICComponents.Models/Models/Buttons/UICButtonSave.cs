using UIComponents.ComponentModels.Defaults;

namespace UIComponents.ComponentModels.Models.Buttons;


/// <summary>
/// The save button triggers the <see cref="UICActionSubmit"/> and has a default appearance.
/// </summary>
public class UICButtonSave : UICButton
{
    public UICButtonSave()
    {
        ButtonText = TranslationDefaults.ButtonSave;
        Color = Colors.ButtonSave;
        this.AddAttribute("class", "btn-save");
    }
    public UICButtonSave(string postLocation) : this()
    {

        OnClick = new UICActionSubmit(postLocation)
        {
            OnSuccess = new UICActionCloseCard()
            {

            }
        };

    }
}
