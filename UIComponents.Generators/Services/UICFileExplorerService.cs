using System.Runtime.CompilerServices;
using UIComponents.Abstractions.Interfaces.FileExplorer;
using UIComponents.Abstractions.Models.FileExplorer;

namespace UIComponents.Generators.Services;

public class UICFileExplorerService : IFileExplorerService
{
    private readonly IFileExplorerPermissionService? _permissionService;
    private readonly IFileExplorerPathMapper _pathMapper;


    public UICFileExplorerService(IFileExplorerPathMapper pathMapper,IFileExplorerPermissionService permissionService = null)
    {
        _permissionService = permissionService;
        _pathMapper = pathMapper;
    }


    #region FileExplorerService


    public Task<string> GetThumbnail(string absolutePath)
    {
        return Task.FromResult(string.Empty);
    }

    public Task<string> GetIcon(string absolutePath)
    {
        return Task.FromResult(string.Empty);
    }


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

                var info = await CreateFileInfoFromDirectoryPath(subDirectory);
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
                var info = await CreateFileInfoFromFilePath(file);

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


    public async Task<UICFileInfo> CreateFileInfoFromFilePath(string filepath)
    {
        if (!File.Exists(filepath))
            throw new ArgumentNullException();

        var info = _pathMapper.GetRelativePath<UICFileInfo>(filepath);
        info.Thumbnail = await GetThumbnail(filepath);
        info.Icon = await GetIcon(filepath);

        var fileInfo = new FileInfo(filepath);
        info.Created = fileInfo.CreationTime;
        info.LastModified = fileInfo.LastWriteTime;
        info.SizeValue = fileInfo.Length;


        info.CanOpen = await HasPermission(p => p.CurrentUserCanOpenFileOrDirectory(filepath));
        info.CanMove = await HasPermission(p => p.CurrentUserCanMoveFileOrDirectory(filepath));
        info.CanDelete = await HasPermission(p => p.CurrentUserCanDeleteFileOrDirectory(filepath));
        info.CanRename = await HasPermission(p => p.CurrentUserCanRenameFileOrDirectory(filepath));

        return info;
    }
    public async Task<UICFileInfo> CreateFileInfoFromDirectoryPath(string filepath)
    {
        if (!filepath.EndsWith("\\"))
            filepath += "\\";
        if (!Directory.Exists(filepath))
            throw new ArgumentNullException();

        var info = _pathMapper.GetRelativePath<UICFileInfo>(filepath);
        info.Thumbnail = await GetThumbnail(filepath);
        info.Icon = await GetIcon(filepath);
        info.DirectoryHasSubdirectories = Directory.GetDirectories(filepath).Length > 0;
        var fileInfo = new DirectoryInfo(filepath);
        info.Created = fileInfo.CreationTime;
        info.LastModified = fileInfo.LastWriteTime;


        info.CanOpen = await HasPermission(p => p.CurrentUserCanOpenFileOrDirectory(filepath));
        info.CanMove = await HasPermission(p => p.CurrentUserCanMoveFileOrDirectory(filepath));
        info.CanDelete = await HasPermission(p => p.CurrentUserCanDeleteFileOrDirectory(filepath));
        info.CanRename = await HasPermission(p => p.CurrentUserCanRenameFileOrDirectory(filepath));

        return info;
    }
    #endregion
}
