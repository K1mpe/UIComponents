namespace UIComponents.ComponentModels.Models.Buttons;

public class UICButtonSendQuestionResponse : UICButton
{
    public UICButtonSendQuestionResponse() : base()
    {

    }
    public UICButtonSendQuestionResponse(string id, string response, ITranslationModel text) : this()
    {
        ButtonText = text;
        var customAction = new UICCustom();
        customAction.Content += $"await Crud.Post('/ToastNotification/PostResponse', {{id:'{id}', response: '{response}'}});";
        customAction.Content += $"taghelper.card.closeCard($(this));";
        OnClick = customAction;
    }
}
