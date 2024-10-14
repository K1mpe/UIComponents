
using static UIComponents.Abstractions.Interfaces.IUICToastNotification;

namespace UIComponents.Abstractions.Models.HtmlResponses;

public class UICToastRNotification : IUICToastNotification
{
    public UICToastRNotification()
    {
        
    }
    public UICToastRNotification(ToastType type, Translatable message, Translatable title = null)
    {
        Type = type;
        Title = title;
        Message = message;
    }
    public IUICToastNotification.ToastType Type {get;set;}

    public Translatable? Title { get; set; }

    public Translatable Message { get; set; }

    public int Duration { get; set; } = 10;
}
