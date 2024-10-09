namespace UIComponents.Models.Models.Actions;

public class UICActionGoBack : UICCustom
{
    public UICActionGoBack(bool forceReload)
    {
        if(forceReload)
        {
            Content = "location = navigation.activation.from?.url ?? '/'";
        }
        else
        {
            Content = "history.back();";
        }
        
    }
}
