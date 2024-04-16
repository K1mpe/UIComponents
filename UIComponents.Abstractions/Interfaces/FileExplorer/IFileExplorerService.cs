using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Models.FileExplorer;

namespace UIComponents.Abstractions.Interfaces.FileExplorer
{
    public interface IFileExplorerService
    {
        Task CopyFilesAsync((RelativePathModel FromPath, RelativePathModel ToPath)[] copyFiles);

        Task DeleteFilesAsync(RelativePathModel[] pathModels);

        Task<GetFilesForDirectoryResultModel> GetFilesFromDirectoryAsync(GetFilesForDirectoryFilterModel filterModel, CancellationToken cancellationToken);

        Task<UICFileInfo> GetFilePreviewAsync(RelativePathModel pathModel, CancellationToken cancellationToken);

        Task MoveFilesAsync((RelativePathModel FromPath, RelativePathModel ToPath)[] moveFiles);

    }
}
