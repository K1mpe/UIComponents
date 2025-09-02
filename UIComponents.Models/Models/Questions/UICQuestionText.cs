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

    public bool Multiline { get; set; }

    public string DefaultValue { get; set; }

    /// <summary>
    /// Validate the text result before submitting.
    /// return a string if there is a validation error
    /// </summary>
    /// <remarks>
    /// Available args: value
    /// </remarks>
    public IUICAction TextValidation { get; set; } = new UICCustom();

    public bool ValidateRequired { get; set; } = true;
    public int? ValidateMinLength { get; set; } 
    public int? ValidateMaxLength { get; set; }


    public override string MapResponse(string response) => response;

    public static UICQuestionText Create(Translatable title, Translatable message, IUICQuestionService questionService)
    {
        var instance = new UICQuestionText(title, message);
        instance.AssignClickEvents(questionService);
        return instance;
    }
}
