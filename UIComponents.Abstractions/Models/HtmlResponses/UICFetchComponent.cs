namespace UIComponents.Abstractions.Models.HtmlResponse;

public class UICFetchComponent : IHtmlResponse
{
    public UICFetchComponent()
    {

    }
    public UICFetchComponent(string key, string appendTo)
    {
        ComponentKey = key;
        AppendTo = appendTo;
    }

    public string Type => nameof(UICFetchComponent);

    public string ComponentKey { get; set; }

    public string AppendTo { get; set; } = "body";
}
