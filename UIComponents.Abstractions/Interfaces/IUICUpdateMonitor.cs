namespace UIComponents.Abstractions.Interfaces;

public interface IUICUpdateMonitor
{
    /// <summary>
    /// This method is called when a file will be changed by updating the UIComponents Nuget. This method can block updating (not recommended) or log which files are changed
    /// </summary>
    /// <param name="filepath">The absolute path in your current project</param>
    /// <param name="newData">Stream with the new data that will be parsed in the file</param>
    /// <param name="overwriteExistingFile">The action that triggers the replacing</param>
    /// <remarks>
    /// Do not forget to call the <paramref name="overwriteExistingFile"/> action to replace the file!
    /// </remarks>
    public void FileWillBeUpdated(string filepath, Stream newData, Action overwriteExistingFile);
}

