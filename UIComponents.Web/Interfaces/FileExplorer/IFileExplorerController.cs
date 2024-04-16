using UIComponents.Abstractions.Models.FileExplorer;

namespace UIComponents.Web.Interfaces.FileExplorer;

public interface IFileExplorerController
{
    /// <summary>
    /// This controllerfunction must return a JSON result with a <see cref="GetFilesForDirectoryResultModel"/>
    /// Can also return any Json response that is supported by the getPost
    /// </summary>
    public Task<IActionResult> GetFilesForDirectory(GetFilesForDirectoryFilterModel fm);

    /// <summary>
    /// Called when trying to open a file, may redirect to <see cref="DownloadFile(RelativePathModel)"/>
    /// </summary>
    /// <param name="pathModel"></param>
    /// <returns></returns>
    public Task<IActionResult> OpenFiles(RelativePathModel pathModel);

    /// <summary>
    /// Get a preview of a file
    /// </summary>
    public Task<IActionResult> Preview(RelativePathModel pathModel);

    public Task<IActionResult> DownloadFile(RelativePathModel pathModel);

    public Task<IActionResult> MoveFiles((RelativePathModel FromPath, RelativePathModel ToPath)[] files);

    public Task<IActionResult> CopyFiles((RelativePathModel FromPath, RelativePathModel ToPath)[] files);

    public Task<IActionResult> DeleteFiles(RelativePathModel[] pathModel);

    public Task<IActionResult> UploadFiles(RelativePathModel directoryPathModel);
}
