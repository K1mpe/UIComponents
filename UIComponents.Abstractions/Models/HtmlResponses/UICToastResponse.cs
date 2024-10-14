namespace UIComponents.Abstractions.Models.HtmlResponses;

public class UICToastResponse : IHtmlResponse
{
    public string Type => "ToastResponse";

    public UICToastResponse()
    {
        
    }
    public UICToastResponse(IUICToastNotification notification)
    {
        Notification = notification;
    }
    public UICToastResponse(IUICToastNotification.ToastType type, Translatable message, Translatable? title= null) : this(new UICToastRNotification(type, message, title))
    {
        
    }

    public IUICToastNotification Notification { get; set; }
    public object? Data { get; set; }
}
