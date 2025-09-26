using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces.FileExplorer;
using UIComponents.Abstractions.Models.FileExplorer;

namespace UIComponents.Generators.Services.FileExplorer.FileInfoManipulators
{
    public class UnknownFileIconFileManipulator : IUICFileExplorerFileInfoManipulator
    {
        public bool AllowFiles { get; set; }

        public bool AllowDirectories { get; set; }

        public double Priority => double.MaxValue;

        private string _unknownIcon;
        public Task Initialize(GetFilesForDirectoryFilterModel filterModel, List<UICFileInfo> fileInfo)
        {
            var path = UICFileExplorerService.FileExplorerImgRoot + "unknown-file.png";
            if(File.Exists(path))
            {
                AllowFiles = true;
                AllowDirectories = true;
                _unknownIcon = UICFileExplorerService.ImgTag(path);
            }
            return Task.CompletedTask;
        }

        public Task<UICFileInfo> ManipulateFileInfo(UICFileInfo fileInfo)
        {
            if (!string.IsNullOrEmpty(fileInfo.Icon) || string.IsNullOrWhiteSpace(fileInfo.Extension))
                return Task.FromResult(fileInfo);

            fileInfo.Icon = _unknownIcon;
            return Task.FromResult(fileInfo);
        }
        public Task Destroy() => Task.CompletedTask;
    }
}
