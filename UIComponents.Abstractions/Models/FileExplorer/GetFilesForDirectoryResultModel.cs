namespace UIComponents.Abstractions.Models.FileExplorer;

public class GetFilesForDirectoryResultModel
{
    public List<UICFileInfo> Files { get; set; } = new();
    public bool CanCreateFileInDirectory { get; set; }
    public bool CanCreateFolderInDirectory { get; set; }

    public bool CanDownloadCurrentDirectory { get; set; }

    /// <summary>
    /// These classes are added to the partial containing the files
    /// </summary>
    public string Classes { get; set; } = string.Empty;

    public GetFilesForDirectoryResultModel AddClass(string className)
    {
        if(Classes.Contains(className)) 
            return this;

        if (string.IsNullOrEmpty(Classes))
            Classes += " ";
        Classes += className;
        return this;
    }

    public RelativePathModel PathModel { get; set; }
}
