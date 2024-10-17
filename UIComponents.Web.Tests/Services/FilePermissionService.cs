using UIComponents.Abstractions.Interfaces.FileExplorer;

namespace UIComponents.Web.Tests.Services;

public class FilePermissionService : IUICFileExplorerPermissionService
{
    public Task<bool> CurrentUserCanCreateDirectory(string path)
    {
        return Task.FromResult(true);
    }

    public Task<bool> CurrentUserCanCreateInThisDirectory(string path)
    {
        return Task.FromResult(true);
    }

    public Task<bool> CurrentUserCanCreateOrEditFile(string path)
    {
        if (File.Exists(path))
            return Task.FromResult(false);
        return Task.FromResult(true);
    }

    public Task<bool> CurrentUserCanDeleteFileOrDirectory(string path)
    {
        return Task.FromResult(true);
    }

    public Task<bool> CurrentUserCanMoveFileOrDirectory(string path)
    {
        return Task.FromResult(true);
    }

    public Task<bool> CurrentUserCanOpenFileOrDirectory(string path)
    {
        return Task.FromResult(true);
    }

    public Task<bool> CurrentUserCanRenameFileOrDirectory(string path, string newFileName)
    {
        return Task.FromResult(true);
    }

    public Task<bool> CurrentUserCanViewFileOrDirectory(string path)
    {
        return Task.FromResult(true);
    }
}
