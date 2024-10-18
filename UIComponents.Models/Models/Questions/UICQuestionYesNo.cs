using UIComponents.Abstractions.Interfaces.Services;

namespace UIComponents.Models.Models.Questions;

public class UICQuestionYesNo : UICQuestionBase<bool>, IUIQuestionComponent<bool>
{

    #region Ctor
    public UICQuestionYesNo() : this(null, null)
    {

    }
    public UICQuestionYesNo(Translatable title, Translatable message) : base(title, message)
    {
        ButtonSubmit.ButtonText = TranslatableSaver.Save("Button.Yes");
        ButtonNo = new UICButton(TranslatableSaver.Save("Button.No"))
        {
            Color = new UICColor("secondary")
        };
    }
    #endregion

    #region Properties

    public UICButton ButtonNo { get; set; }

    [UICIgnoreGetChildrenFunction]
    public UICButton ButtonYes
    {
        get { return ButtonSubmit; }
        set { ButtonSubmit = value; }
    }

    #endregion

    #region Methods

    public static UICQuestionYesNo Create(Translatable title, Translatable message, IUICQuestionService questionService, Action<UICQuestionYesNo> configure = null)
    {
        var instance = new UICQuestionYesNo(title, message);
        instance.AssignClickEvents(questionService);
        if(configure != null)
            configure(instance);
        return instance;
       
    }
    public UICQuestionYesNo AssignClickEvents(IUICQuestionService questionService)
    {
        base.AssignClickEvents(questionService);
        ButtonSubmit.OnClick = new UICActionServerResponse(true, (data) =>
        {
            questionService.AnswerQuestion(Id, "true");
        });
        ButtonNo.OnClick = new UICActionServerResponse(true, (data) =>
        {
            questionService.AnswerQuestion(Id, "false");
        });
        return this;
    }

    public override bool MapResponse(string response)
    {
        return response.ToUpper() == "TRUE";
    }

    #endregion
}
