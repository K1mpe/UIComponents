namespace UIComponents.Abstractions.Models.FileExplorer.Exceptions;

public class UICFileExplorerCannotMoveException : ArgumentStringException
{
    public UICFileExplorerCannotMoveException(string filepath) : base("Cannot move file or directory {0}", filepath)
    {
    }
}
