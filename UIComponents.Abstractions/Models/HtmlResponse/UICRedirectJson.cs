namespace UIComponents.Abstractions.Models.HtmlResponse;

public class UICRedirectJson : IHtmlResponse
{
    public string type => "Redirect";

    public UICRedirectJson(string url)
    {
        this.url = url;
    }

    public string url { get; set; }
}
