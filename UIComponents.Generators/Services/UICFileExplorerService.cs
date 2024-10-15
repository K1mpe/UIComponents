using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces.FileExplorer;
using UIComponents.Abstractions.Models.FileExplorer;
using UIComponents.Abstractions.Models.FileExplorer.Exceptions;
using UIComponents.Abstractions.Varia;
using UIComponents.Generators.Helpers;
using UICFileInfo = UIComponents.Abstractions.Models.FileExplorer.UICFileInfo;

namespace UIComponents.Generators.Services;

public class UICFileExplorerService : IUICFileExplorerService
{
    private readonly IUICFileExplorerPathMapper _pathMapper;
    private readonly ILogger _logger;
    private readonly IUICFileExplorerPermissionChecker _permissionChecker;
    private readonly IUICFileExplorerExecuteActions _executeActions;
    private readonly IUICFileExplorerPermissionService _permissionService;
    public UICFileExplorerService(IUICFileExplorerPathMapper pathMapper, ILogger<UICFileExplorerService> logger, IUICFileExplorerPermissionChecker permissionChecker, IUICFileExplorerExecuteActions executeActions, IUICFileExplorerPermissionService permissionService= null)
    {
        _pathMapper = pathMapper;
        _logger = logger;
        if (!FileExplorerGenerators.Any())
            AddDefaultGenerators();
        _permissionChecker = permissionChecker;
        _executeActions = executeActions;
        _permissionService = permissionService;
    }


    #region FileExplorerService

    public static List<FileExplorerGenerator> FileExplorerGenerators { get; set; } = new();

    #region Actions
    public async virtual Task CreateDirectory(RelativePathModel pathModel)
    {
        var absolutePath = _pathMapper.GetAbsolutePath(pathModel);
        if (Directory.Exists(absolutePath))
            return;

        if (!await HasPermission(p => p.CurrentUserCanCreateDirectory(absolutePath)))
            throw new UICFileExplorerCannotCreateException(absolutePath);
        await _executeActions.CreateDirectoryAsync(absolutePath);
    }
    public async virtual Task CopyFilesAsync(List<RelativePathModel> sourceFiles, RelativePathModel targetDirectory)
    {
        await _permissionChecker.ThrowIfCurrentUserCantCopyFilesAsync(sourceFiles, targetDirectory);
        
        var destination = _pathMapper.GetAbsolutePath(targetDirectory);
        foreach(var copy in sourceFiles)
        {
            string absoluteSource = _pathMapper.GetAbsolutePath(copy);
            string sourceRoot = absoluteSource;
            if(File.Exists(absoluteSource))
            {
                var fileInfo = new FileInfo(absoluteSource);
                sourceRoot = fileInfo.DirectoryName;
                await CopyFile(absoluteSource);
            }

            async Task CopyFile(string sourcePath)
            {
                string absoluteDestination = _pathMapper.ReplaceRoot(sourcePath, sourceRoot, destination);
                using (_logger.BeginScopeKvp(new("FilePath", absoluteDestination), new("FileSource", sourcePath)))
                {
                    if (Directory.Exists(sourcePath) && !Directory.Exists(absoluteDestination))
                    {
                        await _executeActions.CreateDirectoryAsync(absoluteDestination);

                        var subDirectories = Directory.GetDirectories(sourcePath);
                        foreach (var subDirectory in subDirectories)
                        {
                            await CopyFile(subDirectory);
                        }
                        var subFiles = Directory.GetFiles(sourcePath);
                        foreach(var subFile in subFiles)
                        {
                            await CopyFile(subFile);
                        }
                    }
                    else if (File.Exists(sourcePath))
                    {
                        await _executeActions.CopyFileAsync(sourcePath, absoluteDestination);
                    }

                }
            }
        }
    }

    public async virtual Task DeleteFilesAsync(List<RelativePathModel> pathModels)
    {
        await _permissionChecker.ThrowIfCurrentUserCantDeleteFilesAsync(pathModels);
        foreach(var pathModel in pathModels)
        {
            var absolutePath = _pathMapper.GetAbsolutePath(pathModel);
            using(_logger.BeginScopeKvp("FilePath", absolutePath))
            {
                await _executeActions.DeleteFileAsync(absolutePath);
            }
        }
    }

    
    public async virtual Task MoveFilesAsync(List<RelativePathModel> sourceFiles, RelativePathModel targetDirectory)
    {
        await _permissionChecker.ThrowIfCurrentUserCantCopyFilesAsync(sourceFiles, targetDirectory);

        var destination = _pathMapper.GetAbsolutePath(targetDirectory);
        foreach (var copy in sourceFiles)
        {
            string absoluteSource = _pathMapper.GetAbsolutePath(copy);
            string sourceRoot = absoluteSource;
            if (File.Exists(absoluteSource))
            {
                var fileInfo = new FileInfo(absoluteSource);
                sourceRoot = fileInfo.DirectoryName;
                await MoveFile(absoluteSource);
            }

            async Task MoveFile(string sourcePath)
            {
                string absoluteDestination = _pathMapper.ReplaceRoot(sourcePath, sourceRoot, destination);
                using (_logger.BeginScopeKvp(new("FilePath", absoluteDestination), new("FileSource", sourcePath)))
                {
                    if (Directory.Exists(sourcePath) && !Directory.Exists(absoluteDestination))
                    {
                        await _executeActions.CreateDirectoryAsync(absoluteDestination);

                        var subDirectories = Directory.GetDirectories(sourcePath);
                        foreach (var subDirectory in subDirectories)
                        {
                            await MoveFile(subDirectory);
                        }
                        var subFiles = Directory.GetFiles(sourcePath);
                        foreach (var subFile in subFiles)
                        {
                            await MoveFile(subFile);
                        }
                    }
                    else if (File.Exists(sourcePath))
                    {
                        await _executeActions.MoveFileAsync(sourcePath, absoluteDestination);
                    }

                }
            }
        }
    }

    public async virtual Task RenameFileOrDirectoryAsync(RelativePathModel pathModel, string newName)
    {
        await _permissionChecker.ThrowIfCurrentUserCantRenameFileOrDirectory(pathModel, newName);
        var absoluteFile = _pathMapper.GetAbsolutePath(pathModel);
        if (File.Exists(absoluteFile))
        {
            await _executeActions.RenameFileAsync(absoluteFile, newName);
        } else if (Directory.Exists(absoluteFile))
        {
            await _executeActions.RenameDirectoryAsync(absoluteFile, newName);
        }
        
    }

    public async virtual Task DownloadFilesAndDirectories(List<RelativePathModel> pathModels, Stream outputStream)
    {
        if (!pathModels.Any())
            throw new ArgumentNullException(nameof(pathModels));

        if (pathModels.Count() == 1)
        {
            var pathModel = pathModels[0];

            string absolutePath = _pathMapper.GetAbsolutePath(pathModel);
            if (_permissionService != null && !await _permissionService.CurrentUserCanOpenFileOrDirectory(absolutePath))
                throw new AccessViolationException();

            if (System.IO.File.Exists(absolutePath))
            {
                FileInfo fileInfo = new FileInfo(absolutePath);

                using (var fileStream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
                {
                    // Directly copy the file stream to the output stream (the response)
                    await fileStream.CopyToAsync(outputStream);
                }

                // Flush the output stream to ensure the response is sent to the client
                await outputStream.FlushAsync();
                return; // Exit after sending the single file
            }
        }

        // Create the ZipArchive directly in the outputStream (i.e., response stream)
        await _logger.LogFunction("Creating Zip file", true, async () =>
        {
            using (var archive = new ZipArchive(outputStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var pathModel in pathModels)
                {
                    var absolutePath = _pathMapper.GetAbsolutePath(pathModel);

                    if (File.Exists(absolutePath))
                    {
                        var fileInfo = new FileInfo(absolutePath);
                        var fileEntry = archive.CreateEntry(fileInfo.Name);

                        using (var entryStream = fileEntry.Open())
                        using (var fileStream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
                        {
                            await fileStream.CopyToAsync(entryStream);  // Stream each file directly into the ZIP
                        }
                    }
                    else if (Directory.Exists(absolutePath))
                    {
                        var dirInfo = new DirectoryInfo(absolutePath);
                        foreach (var filePath in Directory.GetFiles(absolutePath, "*", SearchOption.AllDirectories))
                        {
                            var relativePath = _pathMapper.ReplaceRoot(filePath, absolutePath, $"{dirInfo.Name}\\");
                            var fileEntry = archive.CreateEntry(relativePath);

                            using (var entryStream = fileEntry.Open())
                            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
                            {
                                await fileStream.CopyToAsync(entryStream);  // Stream each file directly into the ZIP
                            }
                        }
                    }
                }
            }
        }, LogLevel.Information);

        // Flush the output stream to ensure all data is sent to the client
        await outputStream.FlushAsync();
    }
    #endregion

    public async virtual Task<GetFilesForDirectoryResultModel> GetFilesFromDirectoryAsync(GetFilesForDirectoryFilterModel filterModel, CancellationToken cancellationToken)
    {
        string absolutePath = _pathMapper.GetAbsolutePath(filterModel);

        var result = new GetFilesForDirectoryResultModel();

        if (!Directory.Exists(absolutePath))
            return result;

        if (!await HasPermission(p => p.CurrentUserCanOpenFileOrDirectory(absolutePath)))
            throw new AccessViolationException();

        result.CanCreateInDirectory = await HasPermission(p=>p.CurrentUserCanCreateInThisDirectory(absolutePath));

        if (!filterModel.FilesOnly)
        {
            var subDirectories = Directory.GetDirectories(absolutePath);
            foreach (var subDirectory in subDirectories)
            {
                if (!await HasPermission(p => p.CurrentUserCanViewFileOrDirectory(subDirectory)))
                    continue;

                var info = await CreateFileInfoFromDirectoryPath(subDirectory, filterModel);
                result.Files.Add(info);
            }
        }

        if (!filterModel.FoldersOnly)
        {
            var includeSubDirectories = filterModel.FilesOnly ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = Directory.GetFiles(absolutePath, filterModel.Filter ?? "*.*", includeSubDirectories);

            foreach (var file in files)
            {
                if (!await HasPermission(p => p.CurrentUserCanViewFileOrDirectory(file)))
                    continue;
                var info = await CreateFileInfoFromFilePath(file, filterModel);

                result.Files.Add(info);
            }
        }


        return result;
    }

    public async virtual Task<UICFileInfo> CreateFileInfoFromFilePath(string filepath, GetFilesForDirectoryFilterModel filterModel)
    {
        if (!File.Exists(filepath))
            throw new ArgumentNullException();

        var info = _pathMapper.GetRelativePath<UICFileInfo>(filepath);

        var fileInfo = new FileInfo(filepath);
        info.Created = fileInfo.CreationTime;
        info.LastModified = fileInfo.LastWriteTime;
        info.SizeValue = fileInfo.Length;


        info.CanOpen = await HasPermission(p => p.CurrentUserCanOpenFileOrDirectory(filepath));
        info.CanMove = await HasPermission(p => p.CurrentUserCanMoveFileOrDirectory(filepath));
        info.CanDelete = await HasPermission(p => p.CurrentUserCanDeleteFileOrDirectory(filepath));
        info.CanRename = await HasPermission(p => p.CurrentUserCanRenameFileOrDirectory(filepath, null));

        info = await RunFileInfoGeneratorsAsync(info, filterModel, filepath);

        return info;
    }
    public async virtual Task<UICFileInfo> CreateFileInfoFromDirectoryPath(string filepath, GetFilesForDirectoryFilterModel filterModel)
    {
        if (!filepath.EndsWith("\\"))
            filepath += "\\";
        if (!Directory.Exists(filepath))
            throw new ArgumentNullException();

        var info = _pathMapper.GetRelativePath<UICFileInfo>(filepath);
        info.DirectoryHasSubdirectories = Directory.GetDirectories(filepath).Length > 0;
        var fileInfo = new DirectoryInfo(filepath);
        info.Created = fileInfo.CreationTime;
        info.LastModified = fileInfo.LastWriteTime;

        info.SizeValue = 0;
        foreach(var file in fileInfo.GetFiles("*", SearchOption.AllDirectories))
        {
            info.SizeValue += file.Length;
        }

        info.CanOpen = await HasPermission(p => p.CurrentUserCanOpenFileOrDirectory(filepath));
        info.CanMove = await HasPermission(p => p.CurrentUserCanMoveFileOrDirectory(filepath));
        info.CanDelete = await HasPermission(p => p.CurrentUserCanDeleteFileOrDirectory(filepath));
        info.CanRename = await HasPermission(p => p.CurrentUserCanRenameFileOrDirectory(filepath, null));
        info = await RunFileInfoGeneratorsAsync(info, filterModel, filepath);
        return info;
    }

    public async virtual Task<UICFileInfo> GetFilePreviewAsync(RelativePathModel pathModel, CancellationToken cancellationToken)
    {
        var absolutePath = _pathMapper.GetAbsolutePath(pathModel);
        await Task.Delay(0);
        if(File.Exists(absolutePath))
        {
            return await CreateFileInfoFromFilePath(absolutePath, new()
            {
                UseThumbnails = true,
            });
        }else if (Directory.Exists(absolutePath))
        {
            return await CreateFileInfoFromDirectoryPath(absolutePath, new() { 
                UseThumbnails = true, 
            });
        }
        return null;
       
    }
    #endregion

    #region FileExplorerGenerators

    private static string FileExplorerImgRoot => $"{Directory.GetCurrentDirectory()}\\wwwroot\\uic\\img\\file-explorer\\"; 
    private static string ImgTag(string filePathInRoot)
    {
        var relativePath = filePathInRoot.Replace($"{Directory.GetCurrentDirectory()}\\wwwroot", "").Replace("\\", "/");
        return $"<img src=\"{relativePath}\">";
    }
    public virtual void AddGenerator(string name, double priority, Func<UICFileInfo, GetFilesForDirectoryFilterModel, string, Task> function)
    {
        lock (FileExplorerGenerators)
        {
            FileExplorerGenerators.Add(new()
            {
                Name = name,
                Score = priority,
                Function = function
            });
            FileExplorerGenerators = FileExplorerGenerators.OrderBy(x=>x.Score).ToList();
        }
    }
    public void AddGenerator(string name, double priority, Action<UICFileInfo, GetFilesForDirectoryFilterModel, string> action)
    {
        AddGenerator(name, priority, (fileInfo, filterModel, absolutePath)=>
        {
            action.Invoke(fileInfo, filterModel, absolutePath);
            return Task.CompletedTask;
        });
    }
    #endregion

    private async Task<UICFileInfo> RunFileInfoGeneratorsAsync(UICFileInfo fileInfo, GetFilesForDirectoryFilterModel filterModel, string absolutePath)
    {
        var generators = new List<FileExplorerGenerator>();
        lock (FileExplorerGenerators)
        {
            generators = FileExplorerGenerators.OrderBy(x => x.Score).ToList();
        }
        foreach(var generator in generators)
        {
            try
            {
                await generator.Function(fileInfo, filterModel, absolutePath);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
        return fileInfo;
    }

    /// <summary>
    /// Provide this function with the path to the all.css file from font awesome. If .fa-file.[ext] exists, this will be used as icon
    /// </summary>
    /// <param name="fontAwesomeFilePath"></param>
    /// <param name="priority"></param>
    public virtual void UseFontAwesomeIcons(string fontAwesomeFilePath, double priority)
    {
        AddGenerator("FontAwesomeIcons", priority, (fileInfo, filterMode, absolutePath) =>
        {
            if (!string.IsNullOrWhiteSpace(fileInfo.Icon))
                return;

            if(string.IsNullOrEmpty(FontAwesomeCssContent) && File.Exists(fontAwesomeFilePath))
            {
                FontAwesomeCssContent = File.ReadAllText(fontAwesomeFilePath);
            }
            if (FontAwesomeCssContent.Contains($".fa-file-{fileInfo.Extension}"))
                fileInfo.Icon = $"<i class=\"fa-file-{fileInfo.Extension}\"></i>";

        });
    }
    private string FontAwesomeCssContent;

    private void AddDefaultGenerators()
    {
        AddGenerator("ImageThumbnails", 1000, (fileInfo, filterModel, absolutePath) =>{
            if (!filterModel.UseThumbnails)
                return;

            if (!string.IsNullOrEmpty(fileInfo.Thumbnail))
                return;
            switch (fileInfo.Extension.ToUpper())
            {
                case "JPG":
                case "JPEG":
                case "PNG":
                case "BMP":
                    break;
                default:
                    return;
            }


            fileInfo.Thumbnail = $"<img src=\"data:image/png;base64,{CreateThumbnailFromImage.Create(absolutePath, 200, 200)}\">";
        });
        AddGenerator("PdfThumbnail", 1000, (fileInfo, filterModel, absolutePath) =>
        {
            if (!filterModel.UseThumbnails)
                return;
            if (!string.IsNullOrEmpty(fileInfo.Thumbnail))
                return;

            List<string> extensions = new() { "pdf" };
            if (!extensions.Contains(fileInfo.Extension.ToLower()))
                return;

            using (var ms = new MemoryStream())
            {
                CreateThumbnailFromPdf.CreateThumbnail(ms, absolutePath);
                ms.Position = 0;
                var array = ms.ToArray();
                string base64 = Convert.ToBase64String(array);
                if(!string.IsNullOrWhiteSpace(base64))
                    fileInfo.Thumbnail = $"<img src=\"data:image/png;base64,{base64}\">";
            }
        });

        AddGenerator("ImageIcons", 1000, (fileInfo, filterModel, absolutePath) => 
        {
            if (!string.IsNullOrEmpty(fileInfo.Icon))
                return;
            switch (fileInfo.Extension.ToUpper())
            {
                case "JPG":
                case "JPEG":
                case "PNG":
                case "BNP":
                    break;
                default:
                    return;
            }
            if(!fileInfo.Data.ContainsKey("class"))
                fileInfo.Data["class"] = string.Empty;
            fileInfo.Data["class"] += "explorer-img";

            var photoIcon = $"{FileExplorerImgRoot}photo.png";
            if (File.Exists(photoIcon))
                fileInfo.Icon = ImgTag(photoIcon);
        });
        AddGenerator("ExtensionIcons", 1001, (fileInfo, filterModel, absolutePath) =>
        {
            if (!string.IsNullOrEmpty(fileInfo.Icon) || string.IsNullOrWhiteSpace(fileInfo.Extension))
                return;

            var fileExplorerImgs = FileExplorerImgRoot;
            var extensionsImgs = $"{fileExplorerImgs}extensions\\";
            Directory.CreateDirectory(fileExplorerImgs);
            Directory.CreateDirectory(extensionsImgs);

            string filePath = Path.Combine(extensionsImgs, fileInfo.Extension.ToLower());

            string foundExtensionPath = string.Empty;
            if (File.Exists(filePath + ".png"))
                foundExtensionPath = filePath + ".png";
            else if (File.Exists(filePath + ".bnp"))
                foundExtensionPath = filePath + ".bnp";
            else if (File.Exists(filePath + ".jpg"))
                foundExtensionPath = filePath + ".jpg";
            else if (File.Exists(filePath + ".jpeg"))
                foundExtensionPath = filePath + ".jpeg";

            if (string.IsNullOrEmpty(foundExtensionPath))
            {
                if (File.Exists(fileExplorerImgs + "unknown-file.png"))
                    foundExtensionPath = fileExplorerImgs + "unknown-file.png";
            }

            if (!string.IsNullOrEmpty(foundExtensionPath)){
                //string relativePath = foundExtensionPath.Replace(fileExplorerImgs, "/uic/img/fileExplorer/").Replace("\\", "/");
                fileInfo.Icon = ImgTag(foundExtensionPath);
            }
        });
    }

    
    #region Permissions
    public static bool IsDirectory(string filePath, bool throwIfFileOrDirectoryDoesNotExist)
    {
        var directoryInfo = new DirectoryInfo(filePath);
        if (directoryInfo.Exists)
            return true;
        if (throwIfFileOrDirectoryDoesNotExist)
            throw new DirectoryNotFoundException($"{filePath} does not exist");
        return directoryInfo.Extension == string.Empty;
    }
    public static bool IsFile(string filePath, bool throwIfFileOrDirectoryDoesNotExist)
    {
        var fileInfo = new FileInfo(filePath);
        if (fileInfo.Exists)
            return true;
        if (throwIfFileOrDirectoryDoesNotExist)
            throw new DirectoryNotFoundException($"{filePath} does not exist");
        return fileInfo.Extension != string.Empty;
    }

    public Task<bool> HasPermission(Func<IUICFileExplorerPermissionService, Task<bool>> permission)
    {
        if (_permissionService == null)
            return Task.FromResult(true);
        return permission(_permissionService);
    }

    #endregion


}
