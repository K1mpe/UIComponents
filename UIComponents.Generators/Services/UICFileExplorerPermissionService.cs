using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.FileExplorer;

namespace UIComponents.Generators.Services;

public class UICFileExplorerPermissionService : IUICFileExplorerPermissionService
{
    private readonly ILogger<UICFileExplorerPermissionService> _logger;
    public UICFileExplorerPermissionService(ILogger<UICFileExplorerPermissionService> logger)
    {
        _logger = logger;
    }
    private static DateTime? LastNotifyImplementation { get; set; }
    public Task<bool> CurrentUserCanCreateDirectory(string path)
    {
        AlertInvalidImplementation();
        return Task.FromResult(false);
    }

    public Task<bool> CurrentUserCanCreateFileInThisDirectory(string path)
    {
        AlertInvalidImplementation();
        return Task.FromResult(false);
    }

    public Task<bool> CurrentUserCanCreateFolderInThisDirectory(string path)
    {
        AlertInvalidImplementation();
        return Task.FromResult(false);
    }

    public Task<bool> CurrentUserCanCreateOrEditFile(string path)
    {
        AlertInvalidImplementation();
        return Task.FromResult(false);
    }

    public Task<bool> CurrentUserCanDeleteFileOrDirectory(string path)
    {
        AlertInvalidImplementation();
        return Task.FromResult(false);
    }

    public Task<bool> CurrentUserCanMoveFileOrDirectory(string path)
    {
        AlertInvalidImplementation();
        return Task.FromResult(false);
    }

    public Task<bool> CurrentUserCanOpenFileOrDirectory(string path)
    {
        AlertInvalidImplementation();
        return Task.FromResult(true);
    }

    public Task<bool> CurrentUserCanRenameFileOrDirectory(string path, string newFileName)
    {
        AlertInvalidImplementation();
        return Task.FromResult(false);
    }

    public Task<bool> CurrentUserCanViewFileOrDirectory(string path)
    {
        AlertInvalidImplementation();
        return Task.FromResult(true);
    }

    private void AlertInvalidImplementation()
    {
        if(LastNotifyImplementation == null || LastNotifyImplementation < DateTime.Now.AddMinutes(-5))
        {
            _logger.LogError($"There is no implementation for {nameof(IUICFileExplorerPermissionService)}! Please create a implementation to enable file actions");
            LastNotifyImplementation = DateTime.Now;
        }
    }
}
