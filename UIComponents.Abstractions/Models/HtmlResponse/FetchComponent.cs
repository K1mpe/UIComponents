namespace UIComponents.Abstractions.Models.HtmlResponse;

public class FetchComponent : IHtmlResponse
{
    public FetchComponent()
    {

    }
    public FetchComponent(string key)
    {
        ComponentKey = key;
    }

    public string type => nameof(FetchComponent);

    public string ComponentKey { get; set; }

    public object? Arguments { get; set; }

    public string AppendTo { get; set; } = "body";
}
