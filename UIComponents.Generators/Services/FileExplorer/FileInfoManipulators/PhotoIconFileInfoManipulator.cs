using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces.FileExplorer;
using UIComponents.Abstractions.Models.FileExplorer;

namespace UIComponents.Generators.Services.FileExplorer.FileInfoManipulators
{
    public class PhotoIconFileInfoManipulator : IUICFileExplorerFileInfoManipulator
    {
        public bool AllowFiles { get; set; }

        public bool AllowDirectories => false;

        public double Priority => 1000;

        public Task Destroy() => Task.CompletedTask;

        private string _fileIcon = string.Empty;

        public Task Initialize(GetFilesForDirectoryFilterModel filterModel, List<UICFileInfo> fileInfo)
        {
            var photoIcon = $"{UICFileExplorerService.FileExplorerImgRoot}photo.png";
            if (!File.Exists(photoIcon))
            {
                AllowFiles = false;
                return Task.CompletedTask;
            }

            AllowFiles = true;
            _fileIcon = UICFileExplorerService.ImgTag(photoIcon);
            return Task.CompletedTask;
        }

        public Task<UICFileInfo> ManipulateFileInfo(UICFileInfo fileInfo)
        {
            if (!string.IsNullOrEmpty(fileInfo.Icon))
                return Task.FromResult(fileInfo);
            switch (fileInfo.Extension.ToUpper())
            {
                case "JPG":
                case "JPEG":
                case "PNG":
                case "BNP":
                    break;
                default:
                    return Task.FromResult(fileInfo);
            }

            fileInfo.AddClass("explorer-img");
            fileInfo.Icon = _fileIcon;
            

            return Task.FromResult(fileInfo);
        }
    }
}
