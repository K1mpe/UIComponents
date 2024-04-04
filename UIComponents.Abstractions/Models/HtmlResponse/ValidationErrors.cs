namespace UIComponents.Abstractions.Models.HtmlResponse;

public class ValidationErrors : IHtmlResponse
{
    public string type => "ValidationErrors";

    public List<PropertyError> Errors { get; set; } = new();


    public class PropertyError
    {
        public string PropertyName { get; set; }
        public string Error { get; set; }
    }
}
