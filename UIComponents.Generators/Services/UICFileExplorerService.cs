using Microsoft.Extensions.Logging;
using System.IO;
using System.Runtime.CompilerServices;
using UIComponents.Abstractions.Interfaces.FileExplorer;
using UIComponents.Abstractions.Models.FileExplorer;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Services;

public class UICFileExplorerService : IFileExplorerService
{
    private readonly IFileExplorerPermissionService? _permissionService;
    private readonly IFileExplorerPathMapper _pathMapper;
    private readonly ILogger _logger;


    public UICFileExplorerService(IFileExplorerPathMapper pathMapper, ILogger<UICFileExplorerService> logger,IFileExplorerPermissionService permissionService = null)
    {
        _permissionService = permissionService;
        _pathMapper = pathMapper;
        _logger = logger;
        if(!FileExplorerGenerators.Any())
            AddDefaultGenerators();
    }


    #region FileExplorerService

    public static List<FileExplorerGenerator> FileExplorerGenerators { get; set; } = new();


    public Task CopyFilesAsync((RelativePathModel FromPath, RelativePathModel ToPath)[] copyFiles)
    {
        throw new NotImplementedException();
    }

    public Task DeleteFilesAsync(RelativePathModel[] pathModels)
    {
        throw new NotImplementedException();
    }

    public async Task<GetFilesForDirectoryResultModel> GetFilesFromDirectoryAsync(GetFilesForDirectoryFilterModel filterModel, CancellationToken cancellationToken)
    {

        string absolutePath = _pathMapper.GetAbsolutePath(filterModel);

        var result = new GetFilesForDirectoryResultModel();

        if (!Directory.Exists(absolutePath))
            return result;

        if (!await HasPermission(p => p.CurrentUserCanOpenFileOrDirectory(absolutePath)))
            throw new AccessViolationException();

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

        if(!filterModel.FoldersOnly)
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

    public Task MoveFilesAsync((RelativePathModel FromPath, RelativePathModel ToPath)[] moveFiles)
    {
        throw new NotImplementedException();
    }

    Task<bool> HasPermission(Func<IFileExplorerPermissionService, Task<bool>> permissionFunc)
    {
        if (_permissionService == null)
            return Task.FromResult(true);

        return permissionFunc(_permissionService);
    }


    public async Task<UICFileInfo> CreateFileInfoFromFilePath(string filepath, GetFilesForDirectoryFilterModel filterModel)
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
        info.CanRename = await HasPermission(p => p.CurrentUserCanRenameFileOrDirectory(filepath));

        info = await RunFileInfoGeneratorsAsync(info, filterModel, filepath);

        return info;
    }
    public async Task<UICFileInfo> CreateFileInfoFromDirectoryPath(string filepath, GetFilesForDirectoryFilterModel filterModel)
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


        info.CanOpen = await HasPermission(p => p.CurrentUserCanOpenFileOrDirectory(filepath));
        info.CanMove = await HasPermission(p => p.CurrentUserCanMoveFileOrDirectory(filepath));
        info.CanDelete = await HasPermission(p => p.CurrentUserCanDeleteFileOrDirectory(filepath));
        info.CanRename = await HasPermission(p => p.CurrentUserCanRenameFileOrDirectory(filepath));
        info = await RunFileInfoGeneratorsAsync(info, filterModel, filepath);
        return info;
    }

    public async Task<UICFileInfo> GetFilePreviewAsync(RelativePathModel pathModel, CancellationToken cancellationToken)
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
    public void AddGenerator(string name, double priority, Func<UICFileInfo, GetFilesForDirectoryFilterModel, string, Task> function)
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
    public void UseFontAwesomeIcons(string fontAwesomeFilePath, double priority)
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
                case "BNP":
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

    
}
