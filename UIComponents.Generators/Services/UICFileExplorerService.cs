using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces.FileExplorer;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Abstractions.Models.FileExplorer;
using UIComponents.Abstractions.Varia;
using UIComponents.Generators.Helpers;
using UIComponents.Models.Models.Questions;
using UIComponents.Models.Extensions; 
using UICFileInfo = UIComponents.Abstractions.Models.FileExplorer.UICFileInfo;

namespace UIComponents.Generators.Services;

public static class IUICQuestionServiceExtensions
{
    
}

public class UICFileExplorerService : IUICFileExplorerService
{
    private readonly IUICFileExplorerPathMapper _pathMapper;
    private readonly ILogger _logger;
    private readonly IUICFileExplorerExecuteActions _executeActions;
    private readonly IUICFileExplorerPermissionService _permissionService;
    private readonly IUICQuestionService _questionService;
    private readonly IUICLanguageService _languageService;
    public UICFileExplorerService(IUICFileExplorerPathMapper pathMapper,
                                  ILogger<UICFileExplorerService> logger,
                                  IUICFileExplorerExecuteActions executeActions,
                                  IUICQuestionService questionService,
                                  IUICLanguageService languageService,
                                  IUICFileExplorerPermissionService permissionService)
    {
        _pathMapper = pathMapper;
        _logger = logger;
        _executeActions = executeActions;
        _permissionService = permissionService;
        _languageService = languageService;
        _questionService = questionService;

        if (!FileExplorerGenerators.Any())
            AddDefaultGenerators();
    }


    #region FileExplorerService

    public static List<FileExplorerGenerator> FileExplorerGenerators { get; set; } = new();

    #region Actions
    public async virtual Task CreateDirectory(RelativePathModel pathModel)
    {
        var absolutePath = _pathMapper.GetAbsolutePath(pathModel);
        if (Directory.Exists(absolutePath))
            return;

        if (!await _permissionService.CurrentUserCanCreateDirectory(absolutePath))
        {
            _logger.LogError("Cannot create directory {0}");
            return;
        }
        await _executeActions.CreateDirectoryAsync(absolutePath);
    }
    public async virtual Task CopyFilesAsync(List<RelativePathModel> sourceFiles, RelativePathModel targetDirectory)
    {
        List<(string sourcePath, string targetPath)> copyPaths = new();
        List<(string sourcePath, string targetPath)> targetExists = new();
        List<(string sourcePath, string targetPath, bool isFolder)> accessDenied = new();

        var destination = _pathMapper.GetAbsolutePath(targetDirectory);

        foreach (var source in sourceFiles)
        {
            string absoluteSource = _pathMapper.GetAbsolutePath(source);
            string sourceRoot = absoluteSource;
            if (File.Exists(absoluteSource))
            {
                var fileInfo = new FileInfo(absoluteSource);
                sourceRoot = fileInfo.DirectoryName;
            } else if (Directory.Exists(absoluteSource))
            {
                var dirInfo = new DirectoryInfo(absoluteSource);
                sourceRoot = dirInfo.Parent.FullName;
            }
            await ValidateFile(absoluteSource);

            async Task ValidateFile(string sourcePath)
            {
                string absoluteDestination = _pathMapper.ReplaceRoot(sourcePath, sourceRoot, destination);
                using (_logger.BeginScopeKvp(new("FilePath", absoluteDestination), new("FileSource", sourcePath)))
                {
                    if (!await _permissionService.CurrentUserCanViewFileOrDirectory(sourcePath))
                    {
                        _logger.LogDebug("Ignoring {0} from copy list since user cannot view file or directory", sourcePath);
                        return;
                    }
                        

                    if (Directory.Exists(sourcePath)) // is Directory
                    {
                        if (!Directory.Exists(absoluteDestination)) // Target directory does not exist
                        {
                            if(!await _permissionService.CurrentUserCanCreateDirectory(absoluteDestination))
                            {
                                _logger.LogDebug("Access denied to create directory {0}", absoluteDestination);
                                accessDenied.Add(new(sourcePath, absoluteDestination, true));
                                return;
                            }
                        }

                        var subDirectories = Directory.GetDirectories(sourcePath);
                        foreach (var subDirectory in subDirectories)
                        {
                            await ValidateFile(subDirectory);
                        }
                        var subFiles = Directory.GetFiles(sourcePath);
                        foreach (var subFile in subFiles)
                        {
                            await ValidateFile(subFile);
                        }
                    }
                    else if (File.Exists(sourcePath))
                    {
                        if(!await _permissionService.CurrentUserCanOpenFileOrDirectory(sourcePath))
                        {
                            _logger.LogDebug("Access denied to open file {0}", sourcePath);
                            accessDenied.Add(new(sourcePath, absoluteDestination, false));
                            return;
                        }
                        if (File.Exists(absoluteDestination))
                        {
                            _logger.LogDebug("File {0} already exists", absoluteDestination);
                            targetExists.Add(new(sourcePath, absoluteDestination));
                            return;
                        }
                        if(!await _permissionService.CurrentUserCanCreateOrEditFile(absoluteDestination))
                        {
                            _logger.LogDebug("Access denied to create {0}", absoluteDestination);
                            accessDenied.Add(new(sourcePath, absoluteDestination, false));
                            return;
                        }
                        _logger.LogDebug("{0} -> {1} is valid to copy", sourcePath, absoluteDestination);
                        copyPaths.Add(new(sourcePath, absoluteDestination));
                    }
                }
            }
        }

        // Overwrite files or make copy
        if (targetExists.Any())
        {
            var title = TranslatableSaver.Save("FileExplorer.TargetExists.Title", "Target exists");
            var message = TranslatableSaver.Save("FileExplorer.TargetExists.Message", "There are {0} files that already exist. Do you want to replace them?", targetExists.Count);
            if(targetExists.Count == 1)
            {
                var relPath = _pathMapper.GetRelativePath(targetExists.First().targetPath);
                message = TranslatableSaver.Save("FileExplorer.TargetExists.MessageSingle", "{0} already exists. Do you want to replace this item?", relPath);
            }

            var question = UICQuestionYesNo.Create(title, message, _questionService);
            question.ButtonYes.ButtonText = TranslatableSaver.Save("FileExplorer.TargetExists.ReplaceButton", "Replace existing files");
            question.ButtonNo.ButtonText = TranslatableSaver.Save("FileExplorer.TargetExists.Keep both", "Keep both");
            var response = await _questionService.TryAskQuestionToCurrentUser(question, TimeSpan.FromMinutes(1));
            if (!response.IsValid)
                return;

            var copyText = await _languageService.Translate(TranslatableSaver.Save("FileExplorer.TargetExists.UniqueNameSuffix", " (1)"));
            foreach (var file in targetExists)
            {
                var targetPath = file.targetPath;
                if (!response.Result)
                {
                    var fileInfo = new FileInfo(targetPath);
                    targetPath = $"{fileInfo.DirectoryName}\\{fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length)}{copyText}{fileInfo.Extension}";
                }

                if(await _permissionService.CurrentUserCanCreateOrEditFile(targetPath))
                {
                    copyPaths.Add(new(file.sourcePath, targetPath));
                }
                else
                {
                    _logger.LogDebug("Access denied to create {0}", targetPath);
                    accessDenied.Add(new(file.sourcePath, targetPath, false));
                }
            }
        }
        if (!copyPaths.Any())
            throw new AccessViolationException();

        if (accessDenied.Any())
        {
            var folders = accessDenied.Where(x => x.isFolder).Count();
            var files = accessDenied.Where(x=>!x.isFolder).Count();
            var title = TranslatableSaver.Save("FileExplorer.CannotCopy.Title", "Cannot copy files");
            var message = TranslatableSaver.Save("FileExplorer.CannotCopy.MessageFilesAndFolders", "There are {0} files and {1} folders that you cannot copy, do you want to continue?", files, folders);
            if(folders == 0)
                message = TranslatableSaver.Save("FileExplorer.CannotCopy.MessageFiles", "There are {0} files that you cannot copy, do you want to continue?", files);
            else if(files == 0)
                message = TranslatableSaver.Save("FileExplorer.CannotCopy.MessageFolders", "There are {0} folders that you cannot copy, do you want to continue?", files);

            var question = UICQuestionYesNo.Create(title, message, _questionService);
            question.ButtonNo.Render = false;
            var response = await _questionService.TryAskQuestionToCurrentUser(question, TimeSpan.FromMinutes(1));

            if (!response.IsValid || !response.Result)
                return;
        }

        foreach (var file in copyPaths)
        {
            using (_logger.BeginScopeKvp(new("FilePath", file.targetPath), new("FileSource", file.sourcePath)))
            {
                var fileInfo = new FileInfo(file.targetPath);
                var directory = fileInfo.DirectoryName;
                if(!Directory.Exists(directory))
                    await _executeActions.CreateDirectoryAsync(directory);

                await _executeActions.CopyFileAsync(file.sourcePath, file.targetPath);
            }
        }
    }

    public async virtual Task DeleteFilesAsync(List<RelativePathModel> pathModels)
    {
        bool failed = false;
        foreach (var source in pathModels)
        {
            string absoluteSource = _pathMapper.GetAbsolutePath(source);
            string sourceRoot = absoluteSource;
            if (File.Exists(absoluteSource))
            {
                var fileInfo = new FileInfo(absoluteSource);
                sourceRoot = fileInfo.DirectoryName;
                await ValidateFile(absoluteSource);
            }

            async Task ValidateFile(string sourcePath)
            {
                using (_logger.BeginScopeKvp("FilePath", sourcePath))
                {
                    if (!await _permissionService.CurrentUserCanViewFileOrDirectory(sourcePath))
                    {
                        _logger.LogDebug("Access denied to delete a folder containing a file the user cannot view");
                        failed = true;
                        return;
                    }
                    if (!await _permissionService.CurrentUserCanDeleteFileOrDirectory(sourcePath))
                    {
                        _logger.LogDebug("Access denied to delete file or folder {0}", sourcePath);
                        failed = true;
                        return;
                    }

                    if (Directory.Exists(sourcePath)) // is Directory
                    { 
                        var subDirectories = Directory.GetDirectories(sourcePath);
                        foreach (var subDirectory in subDirectories)
                        {
                            await ValidateFile(subDirectory);
                        }
                        var subFiles = Directory.GetFiles(sourcePath);
                        foreach (var subFile in subFiles)
                        {
                            await ValidateFile(subFile);
                        }
                    }
                    else if (File.Exists(sourcePath))
                    {
                        
                        _logger.LogDebug("{0} is valid to delete", sourcePath);
                    }
                }
            }
        }

        foreach (var pathModel in pathModels)
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
        List<(string sourcePath, string targetPath)> movePaths = new();
        List<(string sourcePath, string targetPath)> targetExists = new();
        List<(string sourcePath, string targetPath, bool isFolder)> accessDenied = new();

        var destination = _pathMapper.GetAbsolutePath(targetDirectory);

        foreach (var source in sourceFiles)
        {
            string absoluteSource = _pathMapper.GetAbsolutePath(source);
            string sourceRoot = absoluteSource;
            if (File.Exists(absoluteSource))
            {
                var fileInfo = new FileInfo(absoluteSource);
                sourceRoot = fileInfo.DirectoryName;
            }
            else if (Directory.Exists(absoluteSource))
            {
                var dirInfo = new DirectoryInfo(absoluteSource);
                sourceRoot = dirInfo.Parent.FullName;
            }
            await ValidateFile(absoluteSource);

            async Task ValidateFile(string sourcePath)
            {
                string absoluteDestination = _pathMapper.ReplaceRoot(sourcePath, sourceRoot, destination);
                using (_logger.BeginScopeKvp(new("FilePath", absoluteDestination), new("FileSource", sourcePath)))
                {
                    if (!await _permissionService.CurrentUserCanMoveFileOrDirectory(sourcePath))
                    {
                        _logger.LogDebug("Access denied to move file {0}", sourcePath);
                        accessDenied.Add(new(sourcePath, absoluteDestination, false));
                        return;
                    }

                    if (Directory.Exists(sourcePath)) // is Directory
                    {
                        if (!Directory.Exists(absoluteDestination)) // Target directory does not exist
                        {
                            if (!await _permissionService.CurrentUserCanCreateDirectory(absoluteDestination))
                            {
                                _logger.LogDebug("Access denied to create directory {0}", absoluteDestination);
                                accessDenied.Add(new(sourcePath, absoluteDestination, true));
                                return;
                            }
                        }

                        var subDirectories = Directory.GetDirectories(sourcePath);
                        foreach (var subDirectory in subDirectories)
                        {
                            await ValidateFile(subDirectory);
                        }
                        var subFiles = Directory.GetFiles(sourcePath);
                        foreach (var subFile in subFiles)
                        {
                            await ValidateFile(subFile);
                        }
                    }
                    else if (File.Exists(sourcePath))
                    {
                        if (!await _permissionService.CurrentUserCanOpenFileOrDirectory(sourcePath))
                        {
                            _logger.LogDebug("Access denied to open file {0}", sourcePath);
                            accessDenied.Add(new(sourcePath, absoluteDestination, false));
                            return;
                        }
                        if (File.Exists(absoluteDestination))
                        {
                            _logger.LogDebug("File {0} already exists", absoluteDestination);
                            targetExists.Add(new(sourcePath, absoluteDestination));
                            return;
                        }
                        if (!await _permissionService.CurrentUserCanCreateOrEditFile(absoluteDestination))
                        {
                            _logger.LogDebug("Access denied to create {0}", absoluteDestination);
                            accessDenied.Add(new(sourcePath, absoluteDestination, false));
                            return;
                        }
                        _logger.LogDebug("{0} -> {1} is valid to move", sourcePath, absoluteDestination);
                        movePaths.Add(new(sourcePath, absoluteDestination));
                    }
                }
            }
        }

        // Overwrite files or make copy
        if (targetExists.Any())
        {
            var title = TranslatableSaver.Save("FileExplorer.TargetExists.Title", "Target exists");
            var message = TranslatableSaver.Save("FileExplorer.TargetExists.Message", "There are {0} files that already exist. Do you want to replace them?", targetExists.Count);
            if (targetExists.Count == 1)
            {
                var relPath = _pathMapper.GetRelativePath(targetExists.First().targetPath);
                message = TranslatableSaver.Save("FileExplorer.TargetExists.MessageSingle", "{0} already exists. Do you want to replace this item?", relPath);
            }

            var question = UICQuestionYesNo.Create(title, message, _questionService);
            question.ButtonYes.ButtonText = TranslatableSaver.Save("FileExplorer.TargetExists.ReplaceButton", "Replace existing files");
            question.ButtonNo.ButtonText = TranslatableSaver.Save("FileExplorer.TargetExists.Keep both", "Keep both");
            var response = await _questionService.TryAskQuestionToCurrentUser(question, TimeSpan.FromMinutes(1));
            if(!response.IsValid)
                return;

            var copyText = await _languageService.Translate(TranslatableSaver.Save("FileExplorer.TargetExists.UniqueNameSuffix", " (1)"));
            foreach (var file in targetExists)
            {
                var targetPath = file.targetPath;
                if (!response.Result)
                {
                    var fileInfo = new FileInfo(targetPath);
                    targetPath = $"{fileInfo.DirectoryName}\\{fileInfo.Name}{copyText}{fileInfo.Extension}";
                    if (targetPath.EndsWith("."))
                        targetPath = targetPath.Substring(0, targetPath.Length - 1);
                }

                if (await _permissionService.CurrentUserCanCreateOrEditFile(targetPath))
                {
                    movePaths.Add(new(file.sourcePath, targetPath));
                }
                else
                {
                    _logger.LogDebug("Access denied to create {0}", targetPath);
                    accessDenied.Add(new(file.sourcePath, targetPath, false));
                }
            }
        }
        if (accessDenied.Any())
        {
            var folders = accessDenied.Where(x => x.isFolder).Count();
            var files = accessDenied.Where(x => !x.isFolder).Count();
            var title = TranslatableSaver.Save("FileExplorer.CannotMove.Title", "Cannot move files");
            var message = TranslatableSaver.Save("FileExplorer.CannotMove.MessageFilesAndFolders", "There are {0} files and {1} folders that you cannot move, action is cancelled", files, folders);
            
            var question = UICQuestionYesNo.Create(title, message, _questionService);
            question.ButtonNo.Render = false;
            question.ButtonCancel.Render = false;
            var response = await _questionService.TryAskQuestionToCurrentUser(question, TimeSpan.FromMinutes(1));
            return;
        }

        foreach (var file in movePaths)
        {
            using (_logger.BeginScopeKvp(new("FilePath", file.targetPath), new("FileSource", file.sourcePath)))
            {
                var fileInfo = new FileInfo(file.targetPath);
                var directory = fileInfo.DirectoryName;
                if (!Directory.Exists(directory))
                    await _executeActions.CreateDirectoryAsync(directory);
                await _executeActions.MoveFileAsync(file.sourcePath, file.targetPath);
            }
        }
    }

    public async virtual Task RenameFileOrDirectoryAsync(RelativePathModel pathModel, string newName)
    {
        var absoluteFile = _pathMapper.GetAbsolutePath(pathModel);
        if (!await _permissionService.CurrentUserCanRenameFileOrDirectory(absoluteFile, newName))
            return;

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

        if (!await _permissionService.CurrentUserCanOpenFileOrDirectory(absolutePath))
            throw new AccessViolationException();

        result.CanCreateInDirectory = await _permissionService.CurrentUserCanCreateInThisDirectory(absolutePath);

        if (!filterModel.FilesOnly)
        {
            var subDirectories = Directory.GetDirectories(absolutePath);
            foreach (var subDirectory in subDirectories)
            {
                if (!await _permissionService.CurrentUserCanViewFileOrDirectory(subDirectory))
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
                if (!await _permissionService.CurrentUserCanViewFileOrDirectory(file))
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


        info.CanOpen = await _permissionService.CurrentUserCanOpenFileOrDirectory(filepath);
        info.CanMove = await _permissionService.CurrentUserCanMoveFileOrDirectory(filepath);
        info.CanDelete = await _permissionService.CurrentUserCanDeleteFileOrDirectory(filepath);
        info.CanRename = await _permissionService.CurrentUserCanRenameFileOrDirectory(filepath, null);

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

        info.CanOpen = await _permissionService.CurrentUserCanOpenFileOrDirectory(filepath);
        info.CanMove = await _permissionService.CurrentUserCanMoveFileOrDirectory(filepath);
        info.CanDelete = await _permissionService.CurrentUserCanDeleteFileOrDirectory(filepath);
        info.CanRename = await _permissionService.CurrentUserCanRenameFileOrDirectory(filepath, null);
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

    public Task<IUIComponent?> GetUploadFileComponent(RelativePathModel pathModel)
    {
        return Task.FromResult<IUIComponent?>(null);
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
                //CreateThumbnailFromPdf.CreateThumbnail(ms, absolutePath);
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

    


    #endregion


}
