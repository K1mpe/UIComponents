using UIComponents.Abstractions.Interfaces.FileExplorer;

namespace UIComponents.Abstractions.Models.FileExplorer;

public class GetFilesForDirectoryFilterModel : IRelativePath
{
    public string AbsolutePathReference { get; set; }

    public string RelativePath { get; set; }

    public string Filter { get; set; }

    public bool FoldersOnly { get; set; }

    /// <summary>
    /// If true, no folders will be displayed but all files in the subfolders are available.
    /// </summary>
    public bool FilesOnly { get; set; }
}
