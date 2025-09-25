using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces.FileExplorer;
using UIComponents.Abstractions.Models.FileExplorer;

namespace UIComponents.Generators.Services.FileExplorer.FileInfoManipulators
{
    public class ExtensionIconFileManipulator : IUICFileExplorerFileInfoManipulator
    {
        public bool AllowFiles { get; set; }

        public bool AllowDirectories { get; set; }

        public double Priority => 1000;

        private Dictionary<string, string> extensionsNames = new Dictionary<string, string>();
        private string folderPath = string.Empty;
        public Task Initialize(GetFilesForDirectoryFilterModel filterModel, List<UICFileInfo> fileInfo)
        {
            var fileExplorerImgs = UICFileExplorerService.FileExplorerImgRoot;
            var extensionsImgs = $"{fileExplorerImgs}extensions\\";
            Directory.CreateDirectory(fileExplorerImgs);
            Directory.CreateDirectory(extensionsImgs);

            var files = Directory.GetFiles(extensionsImgs);
            foreach (var file in files)
            {
                var fi = new FileInfo(file);
                var fileName = fi.Name.ToLower().Substring(0, fi.Name.Length - fi.Extension.Length);
                extensionsNames[fileName] = UICFileExplorerService.ImgTag(file);
            }

            AllowFiles = extensionsNames.Any();
            AllowDirectories = extensionsNames.ContainsKey("folder");

            return Task.CompletedTask;
        }

        public Task<UICFileInfo> ManipulateFileInfo(UICFileInfo fileInfo)
        {
            if (!string.IsNullOrEmpty(fileInfo.Icon) || string.IsNullOrWhiteSpace(fileInfo.Extension))
                return Task.FromResult(fileInfo);

            if (fileInfo.IsFolder)
            {
                fileInfo.Icon = extensionsNames["folder"];
                return Task.FromResult(fileInfo);
            }

            var ext = fileInfo.Extension.ToLower();
            if(extensionsNames.ContainsKey(ext))
                fileInfo.Icon = extensionsNames[ext];

            return Task.FromResult(fileInfo);
        }

        public Task Destroy()
        {
            return Task.CompletedTask;
        }
    }
}
