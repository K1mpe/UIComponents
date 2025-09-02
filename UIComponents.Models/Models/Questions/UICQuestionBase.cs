using UIComponents.Abstractions.Interfaces.Services;

namespace UIComponents.Models.Models.Questions;

public abstract class UICQuestionBase : IUIQuestionComponent, IUIComponent
{
    #region Fields
    public virtual string RenderLocation => this.CreateDefaultIdentifier(Renderer);
    public virtual string Id { get; set; } = Guid.NewGuid().ToString("n");
    #endregion

    #region Ctor
    public UICQuestionBase()
    {
        ButtonSubmit = new UICButton(TranslatableSaver.Save("Button.Send"))
        {
            Color = ColorDefaults.ButtonSubmit?.Invoke()
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

    /// <summary>
    /// If false, these options are ingored : 
    /// <br><see cref="ShowClosebutton"/></br>, 
    /// <br><see cref="ButtonCancel"/></br>, 
    /// <br><see cref="CanClickOutSideModalToClose"/></br>
    /// </summary>
    public bool CanCancel { get; set; } = true;
    public bool ShowClosebutton { get; set; } = true;
    public UICButton ButtonSubmit { get; set; }
    public UICButton ButtonCancel { get; set; }

    public QuestionIconType? Icon { get; set; }

    public TimeSpan? RemoveAfterTimeout { get; set; }
    /// <summary>
    /// This option is only available if <see cref="ButtonCancel"/>.Render is true
    /// </summary>
    public bool CanClickOutSideModalToClose { get; set; } = true;

    public string DebugIdentifier => Title?.ToString()??Message.ToString()??string.Empty;


    public UICQuestionRenderer Renderer { get; set; } = UICQuestionRenderer.Modal;
    #endregion

    #region Methods
    protected void AssignClickEvents(IUICQuestionService questionService)
    {
        ButtonSubmit.OnClick = new UICActionServerResponse(true, (data) =>
        {
            questionService.AnswerQuestion(Id, data["value"] ?? null);
        })
        { GetVariableData = new UICCustom("result"), IgnoreKeyNotFound = true };
        ButtonCancel.OnClick = new UICActionServerResponse(true, (data) =>
        {
            questionService.CancelQuestion(Id);
        })
        { IgnoreKeyNotFound = true};
    }
    #endregion
}
public abstract class UICQuestionBase<T> : UICQuestionBase, IUIQuestionComponent<T>
{
    public UICQuestionBase(Translatable title, Translatable message) : base(title,message)
    {

    }
    public abstract T MapResponse(string response);
}
public enum QuestionIconType
{
    Success,
    Error,
    Warning,
    Info,
    Question
}