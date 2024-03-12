namespace UIComponents.Models.Models.Buttons;

public class UICButtonDelete : UICButton
{
    public UICButtonDelete(string controller, object id)
    {
        ButtonText = TranslationDefaults.ButtonDelete;
        Color = ColorDefaults.ButtonDelete;
        OnClick = new UICCustom()
        {
            Content = $"await uic.form.delete('{controller}', {id});"
        };
        PrependButtonIcon = UICIcon.Delete();
        this.AddAttribute("class", "btn-delete");
    }
}
