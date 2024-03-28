
namespace UIComponents.Models.Models.Buttons;

public class UICButtonCreate : UICButton
{
    public UICButtonCreate(Type type, bool modal = false)
    {
        ButtonText = TranslationDefaults.ButtonCreate;

        if (type == null)
            return;

        Tooltip = TranslationDefaults.ButtonCreateTooltip(type);

        if (modal)
        {
            OnClick = new UICActionGetPost(UICActionGetPost.ActionTypeEnum.Get, type.Name, "Create", new { modalTitle = "" })
            {
                OnSuccess = new UICActionOpenResultAsModal()
            };
        }
        else
        {
            OnClick = new UICActionNavigate($"/{type.Name}/Create");
        }
    }

}
