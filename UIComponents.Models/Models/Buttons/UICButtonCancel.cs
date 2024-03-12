namespace UIComponents.Models.Models.Buttons;

public class UICButtonCancel : UICButton
{
    public UICButtonCancel() : base(TranslationDefaults.ButtonCancel)
    {
        OnClick = new UICActionCloseModal()
        {
            OnFailed = new UICActionGoBack()
        };

        AddAttributeToDictionary("type", "reset", Attributes);
    }
}
