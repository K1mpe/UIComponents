namespace UIComponents.Abstractions.Models.HtmlResponse;

public class ValidationErrors : IHtmlResponse
{
    public string type => "ValidationErrors";

    public List<PropertyError> Errors { get; set; } = new();

    public string Blub => "Hallo";
    public int Number => 4;

    public class PropertyError
    {
        public string PropertyName { get; set; }
        public string Error { get; set; }
    }
}
