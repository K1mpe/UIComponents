using UIComponents.Abstractions.Interfaces.Services;

namespace UIComponents.Models.Models.Questions;

public abstract class UICQuestionBase : IUIQuestionComponent, IUIComponent
{
    #region Fields
    public virtual string RenderLocation => this.CreateDefaultIdentifier();
    public virtual string Id { get; set; } = Guid.NewGuid().ToString("n");
    #endregion

    #region Ctor
    public UICQuestionBase()
    {
        ButtonSubmit = new UICButton(new("Button.Send"))
        {
            Color = ColorDefaults.ButtonSave
        };
        ButtonCancel = new UICButton(TranslationDefaults.ButtonCancel);
    }
    public UICQuestionBase(Translatable title, Translatable message) : this()
    {
        Title = title;
        Message = message;        
    }
    #endregion

    #region Properties
    public Translatable Title { get; set; }
    public Translatable Message { get; set; }

    public bool InvertButtons { get; set; }
    public UICButton ButtonSubmit { get; set; }
    public UICButton ButtonCancel { get; set; }

    public QuestionIconType? Icon { get; set; }

    /// <summary>
    /// This option is only available if <see cref="ButtonCancel"/>.Render is true
    /// </summary>
    public bool CanClickOutSideModalToClose { get; set; } = true;
    #endregion

    #region Methods
    protected void AssignClickEvents(IUICQuestionService questionService)
    {
        ButtonSubmit.OnClick = new UICActionServerResponse(true, (data) =>
        {
            questionService.AnswerQuestion(Id, data["value"] ?? null);
        }){ GetVariableData = new UICCustom("result") };
        ButtonCancel.OnClick = new UICActionServerResponse(true, (data) =>
        {
            questionService.CancelQuestion(Id);
        });
    }
    #endregion
}
public enum QuestionIconType
{
    Success,
    Error,
    Warning,
    Info,
    question
}