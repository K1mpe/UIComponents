namespace UIComponents.Abstractions.Models.HtmlResponse;

public class UICValidationErrors : IHtmlResponse
{
    public string Type => "ValidationErrors";

    /// <summary>
    /// The errors for each property
    /// </summary>
    public List<PropertyError> Errors { get; set; } = new();

    /// <summary>
    /// when there is a form with the same Url as action on the page, it will try to find only the validation messages inside this form
    /// </summary>
    public string Url { get; set; }


    public class PropertyError
    {
        public string PropertyName { get; set; }
        public string Error { get; set; }
    }

}
