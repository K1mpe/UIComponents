namespace UIComponents.Abstractions.Models.FileExplorer;

public class GetFilesForDirectoryResultModel
{
    public List<UICFileInfo> Files { get; set; } = new();
    public bool CanCreateFileInDirectory { get; set; }
    public bool CanCreateFolderInDirectory { get; set; }

}
