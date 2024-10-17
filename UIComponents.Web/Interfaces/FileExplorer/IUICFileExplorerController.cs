using UIComponents.Abstractions.Interfaces.FileExplorer;
using UIComponents.Abstractions.Models.FileExplorer;
using UIComponents.Web.Helpers;

namespace UIComponents.Web.Interfaces.FileExplorer
{
    public interface IUICFileExplorerController
    {
        /// <summary>
        /// Copy files from one path to another, the fromPath may contain multiple files or folders
        /// </summary>
        /// <remarks>
        /// Should use <see cref="IUICFileExplorerService.CopyFilesAsync(List{RelativePathModel}, RelativePathModel)"/>
        /// </remarks>
        Task<IActionResult> CopyFiles(RelativePathModel[] FromPath, RelativePathModel ToPath);

        /// <summary>
        /// Create a new directory
        /// </summary>
        /// <remarks>
        /// Should use <see cref="IUICFileExplorerService.CreateDirectory(RelativePathModel)"/>
        /// </remarks>
        Task<IActionResult> CreateDirectory(RelativePathModel pathModel);

        /// <summary>
        /// Delete one or more files
        /// </summary>
        /// <remarks>
        /// Should use <see cref="IUICFileExplorerService.DeleteFilesAsync(List{RelativePathModel})"/>
        /// </remarks>
        Task<IActionResult> DeleteFiles(RelativePathModel[] pathModel);

        /// <summary>
        /// Download one or more files. If more files are selected (or a folder), this should return a zip file.
        /// </summary>
        /// <remarks>
        /// This method may use <see cref="UICFileExplorerHelper.DownloadFileOrZipStream(IEnumerable{string}, Microsoft.AspNetCore.Http.HttpContext, Microsoft.Extensions.Logging.ILogger?, Microsoft.Extensions.Logging.LogLevel, Microsoft.Extensions.Logging.LogLevel, System.IO.Compression.CompressionLevel)"/> for easy implementation
        /// </remarks>
        Task<IActionResult> Download(RelativePathModel[] pathModels);

        /// <summary>
        /// Uses the <see cref="IUICFileExplorerService.GetFilesFromDirectoryAsync(GetFilesForDirectoryFilterModel, CancellationToken)"/> and returns a view
        /// </summary>
        Task<IActionResult> GetFilesForDirectoryJson(GetFilesForDirectoryFilterModel fm);

        /// <summary>
        /// Uses the <see cref="IUICFileExplorerService.GetFilesFromDirectoryAsync(GetFilesForDirectoryFilterModel, CancellationToken)"/> and returns the <see cref="GetFilesForDirectoryResultModel"/> as Json
        /// </summary>
        Task<IActionResult> GetFilesForDirectoryPartial(GetFilesForDirectoryFilterModel fm);

        /// <summary>
        /// Move files from one path to another, the fromPath may contain multiple files or folders
        /// </summary>
        /// <remarks>
        /// Should use <see cref="IUICFileExplorerService.MoveFilesAsync(List{RelativePathModel}, RelativePathModel)"/>
        /// </remarks>
        Task<IActionResult> MoveFiles(RelativePathModel[] FromPath, RelativePathModel ToPath);

        /// <summary>
        /// A Get request to open a file, the base64 string can be converted and deserialised to <see cref="RelativePathModel"/>
        /// </summary>
        Task<IActionResult> OpenFile(string base64);

        /// <summary>
        /// Returns a modal that can be used as a image viewer
        /// </summary>
        Task<IActionResult> OpenImage(RelativePathModel pathModel, string explorerId);

        /// <summary>
        /// A partial that renders a preview of the given file. If no preview is available,the icon is used
        /// </summary>
        Task<IActionResult> Preview(RelativePathModel pathModel);
        Task<IActionResult> Rename(RelativePathModel pathModel, string newName);

        /// <summary>
        /// The post that uploads one or more files
        /// </summary>
        /// <remarks>
        /// Use <see cref="UICFileExplorerHelper.UploadFilesFromDropzoneStream(Microsoft.AspNetCore.Http.HttpContext, string, Microsoft.Extensions.Logging.ILogger)"/> for easy implementation
        /// </remarks>
        /// <param name="directoryPathModel"></param>
        /// <returns></returns>
        Task<IActionResult> UploadFiles(RelativePathModel directoryPathModel);

        /// <summary>
        /// Returns a modal where the user can upload files
        /// </summary>
        /// <param name="directoryPathModel"></param>
        /// <returns></returns>
        Task<IActionResult> UploadPartial(RelativePathModel directoryPathModel);
    }
}