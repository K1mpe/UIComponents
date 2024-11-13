using UIComponents.Generators.Configuration;

namespace UIComponents.Generators.Services;

public class UICUpdateMonitor : IUICUpdateMonitor
{
    private readonly UicConfigOptions uicConfigOptions;

    public UICUpdateMonitor(UicConfigOptions uicConfigOptions)
    {
        this.uicConfigOptions = uicConfigOptions;
    }

    public void FileWillBeUpdated(string filepath, Stream newData, Action overwriteExistingFile)
    {
        if (uicConfigOptions.UpdateMonitorAction != null)
            uicConfigOptions.UpdateMonitorAction?.Invoke(filepath, newData, overwriteExistingFile);
        else
            overwriteExistingFile();
    }
}
