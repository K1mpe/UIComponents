namespace UIComponents.ComponentModels.Models;

public class UICPartialContent : UICCard
{
    public UICPartialContent(string source)
    {
        Source = source;
        OnlyDisplayContent = true;
    }
}
