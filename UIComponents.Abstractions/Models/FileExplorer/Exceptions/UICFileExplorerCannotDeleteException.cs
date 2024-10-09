namespace UIComponents.Abstractions.Models.FileExplorer.Exceptions;

public class UICFileExplorerCannotDeleteException : ArgumentStringException
{
    public UICFileExplorerCannotDeleteException(string filepath) : base("Cannot remove {0}", filepath)
    {
    }
}
