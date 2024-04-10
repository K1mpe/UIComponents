namespace UIComponents.Abstractions.Models.FileExplorer;

public class GetFilesForDirectoryResultModel
{
    public List<UICFileInfo> Files { get; set; } = new();
    public bool CanCreateInDirectory { get; set; }

}
