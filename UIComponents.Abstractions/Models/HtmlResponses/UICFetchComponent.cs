namespace UIComponents.Abstractions.Models.HtmlResponse;

public class UICFetchComponent : IHtmlResponse
{
    public UICFetchComponent()
    {

    }
    public UICFetchComponent(string key)
    {
        ComponentKey = key;
    }

    public string Type => nameof(UICFetchComponent);

    public string ComponentKey { get; set; }

    public object? Arguments { get; set; }

    public string AppendTo { get; set; } = "body";
}
