namespace UIComponents.Abstractions.Interfaces;

public interface IUICToastNotification
{
    public ToastType Type { get; }

    public Translatable? Title { get; }

    public Translatable Message { get; }

    public int Duration { get; }

    public enum ToastType
    {
        Success = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
    }
}
