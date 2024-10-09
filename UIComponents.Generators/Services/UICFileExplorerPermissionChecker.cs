using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces.FileExplorer;
using UIComponents.Abstractions.Models.FileExplorer;
using UIComponents.Abstractions.Models.FileExplorer.Exceptions;

namespace UIComponents.Generators.Services
{
    public class UICFileExplorerPermissionChecker : IUICFileExplorerPermissionChecker
    {
        private readonly ILogger _logger;
        private readonly IUICFileExplorerPathMapper _pathMapper;
        private readonly IUICFileExplorerPermissionService _permissionService;

        public UICFileExplorerPermissionChecker(ILogger<UICFileExplorerPermissionChecker> logger, IUICFileExplorerPathMapper pathMapper, IUICFileExplorerPermissionService permissionService = null)
        {
            _logger = logger;
            _permissionService = permissionService;
            _pathMapper = pathMapper;
        }

        public async Task ThrowIfCurrentUserCantCopyFilesAsync(List<RelativePathModel> sourceFiles, RelativePathModel destinationPath)
        {
            if (_permissionService == null)
                return;

            var destinationRoot = _pathMapper.GetAbsolutePath(destinationPath);
            if (!await _permissionService.CurrentUserCanCreateInThisDirectory(destinationRoot))
                throw new UICFileExplorerCannotCreateInDirectoryException(destinationRoot);

            foreach (var pathModel in sourceFiles)
            {
                var absoluteSource = _pathMapper.GetAbsolutePath(pathModel);
                string sourceRoot = absoluteSource;
                if (File.Exists(absoluteSource))
                {
                    var fileInfo = new FileInfo(absoluteSource);
                    sourceRoot = fileInfo.DirectoryName;
                }
                await ValidateCopy(absoluteSource);

                async Task ValidateCopy(string sourcePath)
                {
                    string absoluteDestination = _pathMapper.ReplaceRoot(sourcePath, sourceRoot, destinationRoot);
                    using (_logger.BeginScopeKvp(new("FilePath", absoluteDestination), new("FileSource", sourcePath)))
                    {
                        if (!Exists(sourcePath))
                            throw new FileNotFoundException(null, sourcePath);

                        if (Directory.Exists(sourcePath) && !Directory.Exists(absoluteDestination))
                        {
                            if (!await _permissionService.CurrentUserCanCreateDirectory(absoluteDestination))
                                throw new UICFileExplorerCannotCreateException(absoluteDestination);

                            var subDirectories = Directory.GetDirectories(sourcePath);
                            foreach (var subDirectory in subDirectories)
                            {
                                await ValidateCopy(subDirectory);
                            }
                            var subFiles = Directory.GetFiles(sourcePath);
                            foreach (var subFile in subFiles)
                            {
                                await ValidateCopy(subFile);
                            }
                        }
                        else if (File.Exists(sourcePath))
                        {
                            if (!await _permissionService.CurrentUserCanCreateOrEditFile(absoluteDestination))
                                throw new UICFileExplorerCannotCreateException(absoluteDestination);
                        }

                    }
                }
            }
        }

        public async Task ThrowIfCurrentUserCantDeleteFilesAsync(List<RelativePathModel> pathModels)
        {
            if (_permissionService == null)
                return;

            foreach(var pathModel in pathModels)
            {
                var absolutePath = _pathMapper.GetAbsolutePath(pathModel);
                if (!Exists(absolutePath))
                    throw new FileNotFoundException(null, absolutePath);
                using (_logger.BeginScopeKvp("FilePath", absolutePath))
                {
                    if (!await _permissionService.CurrentUserCanDeleteFileOrDirectory(absolutePath))
                        throw new UICFileExplorerCannotDeleteException(absolutePath);
                }
            }
        }

        public async Task ThrowIfCurrentUserCantMoveFilesAsync(List<RelativePathModel> sourceFiles, RelativePathModel destinationPath)
        {
            if (_permissionService == null)
                return;

            var destinationRoot = _pathMapper.GetAbsolutePath(destinationPath);
            if (!await _permissionService.CurrentUserCanCreateInThisDirectory(destinationRoot))
                throw new UICFileExplorerCannotCreateInDirectoryException(destinationRoot);

            foreach(var pathModel in sourceFiles)
            {
                var absoluteSource = _pathMapper.GetAbsolutePath(pathModel);
                string sourceRoot = absoluteSource;
                if (File.Exists(absoluteSource))
                {
                    var fileInfo = new FileInfo(absoluteSource);
                    sourceRoot = fileInfo.DirectoryName;
                }
                await ValidateMove(absoluteSource);

                async Task ValidateMove(string sourcePath)
                {
                    string absoluteDestination = _pathMapper.ReplaceRoot(sourcePath, sourceRoot, destinationRoot);
                    using (_logger.BeginScopeKvp(new("FilePath", absoluteDestination), new("FileSource", sourcePath)))
                    {
                        if (!Exists(sourcePath))
                            throw new FileNotFoundException(null, sourcePath);

                        if (Directory.Exists(sourcePath) && !Directory.Exists(absoluteDestination))
                        {
                            if(!await _permissionService.CurrentUserCanCreateDirectory(absoluteDestination))
                                throw new UICFileExplorerCannotCreateException(absoluteDestination);

                            var subDirectories = Directory.GetDirectories(sourcePath);
                            foreach (var subDirectory in subDirectories)
                            {
                                await ValidateMove(subDirectory);
                            }
                            var subFiles = Directory.GetFiles(sourcePath);
                            foreach (var subFile in subFiles)
                            {
                                await ValidateMove(subFile);
                            }
                        }
                        else if (File.Exists(sourcePath))
                        {
                            if(!await _permissionService.CurrentUserCanMoveFileOrDirectory(sourcePath))
                                throw new UICFileExplorerCannotMoveException(sourcePath);

                            if (!await _permissionService.CurrentUserCanCreateOrEditFile(absoluteDestination))
                                throw new UICFileExplorerCannotCreateException(absoluteDestination);
                        }

                    }
                }
            }
        }

        public async Task ThrowIfCurrentUserCantRenameFileOrDirectory(RelativePathModel sourcePath, string newFilename)
        {
            if(string.IsNullOrWhiteSpace(newFilename))
                throw new ArgumentNullException(nameof(newFilename));

            if (_permissionService == null)
                return;
            var absolutePath = _pathMapper.GetAbsolutePath(sourcePath);
            using (_logger.BeginScopeKvp(new("FilePath", absolutePath), new("NewName", newFilename)))
            {
                if (!await _permissionService.CurrentUserCanRenameFileOrDirectory(absolutePath, newFilename))
                    throw new UICFileExplorerCannotRenameException(absolutePath, newFilename);
            }
        }

        #region Helper Methods
        public bool Exists(string path) => File.Exists(path) || Directory.Exists(path);
        public bool IsDirectory(string path)
        {
            var directoryInfo = new DirectoryInfo(path);
            if (directoryInfo.Exists)
                return true;
            return directoryInfo.Extension == string.Empty;
        }
        public bool IsFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
                return true;

            return fileInfo.Extension != string.Empty;
        }
        #endregion
    }
}
