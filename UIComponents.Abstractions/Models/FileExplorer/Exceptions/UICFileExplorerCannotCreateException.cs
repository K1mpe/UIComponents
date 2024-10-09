namespace UIComponents.Abstractions.Models.FileExplorer.Exceptions;

public class UICFileExplorerCannotCreateException : ArgumentStringException
{
    public UICFileExplorerCannotCreateException(string filepath) : base("Cannot create this file or directory: {0}", filepath)
    {
    }
}
