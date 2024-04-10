namespace UIComponents.Abstractions.Interfaces.FileExplorer;

public interface IRelativePath
{
    public string AbsolutePathReference { get; set; }
    public string RelativePath { get; set; }
}
