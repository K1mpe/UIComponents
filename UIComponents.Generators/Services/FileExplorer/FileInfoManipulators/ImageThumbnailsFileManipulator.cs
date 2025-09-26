using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces.FileExplorer;
using UIComponents.Abstractions.Models.FileExplorer;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Services.FileExplorer.FileInfoManipulators
{
    public class ImageThumbnailsFileManipulator : IUICFileExplorerFileInfoManipulator
    {
        public bool AllowFiles { get; set; }

        public bool AllowDirectories => false;

        public double Priority => 1000;

        public Task Destroy() => Task.CompletedTask;

        public Task Initialize(GetFilesForDirectoryFilterModel filterModel, List<UICFileInfo> fileInfo)
        {
            AllowFiles = filterModel.UseThumbnails;

            return Task.CompletedTask;
        }

        public Task<UICFileInfo> ManipulateFileInfo(UICFileInfo fileInfo)
        {
            if (!string.IsNullOrEmpty(fileInfo.Thumbnail))
                return Task.FromResult(fileInfo);

            switch (fileInfo.Extension.ToUpper())
            {
                case "JPG":
                case "JPEG":
                case "PNG":
                case "BMP":
                    break;
                default:
                    return Task.FromResult(fileInfo);
            }


            fileInfo.Thumbnail = $"<img src=\"data:image/png;base64,{CreateThumbnailFromImage.Create(fileInfo.FileInfo.FullName, 200, 200)}\">";
            return Task.FromResult(fileInfo);
        }
    }
}
