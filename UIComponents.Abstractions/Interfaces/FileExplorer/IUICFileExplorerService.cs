using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Models.FileExplorer;

namespace UIComponents.Abstractions.Interfaces.FileExplorer
{
    public interface IUICFileExplorerService
    {
        Task CreateDirectory(RelativePathModel pathModel);
        Task CopyFilesAsync(List<RelativePathModel> sourceFiles, RelativePathModel targetDirectory);

        Task DeleteFilesAsync(List<RelativePathModel> pathModels);

        Task<GetFilesForDirectoryResultModel> GetFilesFromDirectoryAsync(GetFilesForDirectoryFilterModel filterModel, CancellationToken cancellationToken);

        Task<UICFileInfo> GetFilePreviewAsync(RelativePathModel pathModel, CancellationToken cancellationToken);

        Task MoveFilesAsync(List<RelativePathModel> sourceFiles, RelativePathModel targetDirectory);

        Task RenameFileOrDirectoryAsync(RelativePathModel pathModel, string newName);

        Task DownloadFilesAndDirectories(List<RelativePathModel> pathModels, Stream outputStream);
    }
}
