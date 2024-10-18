using UIComponents.Abstractions.Interfaces.Services;

namespace UIComponents.Models.Models.Questions;

public class UICQuestionText : UICQuestionBase<string>
{
    public UICQuestionText() : this(null, null)
    {
        
    }
    public UICQuestionText(Translatable title, Translatable message) : base(title, message)
    {
    }

    public override string MapResponse(string response) => response;

    public static UICQuestionText Create(Translatable title, Translatable message, IUICQuestionService questionService)
    {
        var instance = new UICQuestionText(title, message);
        instance.AssignClickEvents(questionService);
        return instance;
    }
}
