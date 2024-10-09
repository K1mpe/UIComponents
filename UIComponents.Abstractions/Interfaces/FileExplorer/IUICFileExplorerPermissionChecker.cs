using UIComponents.Abstractions.Models.FileExplorer;

namespace UIComponents.Abstractions.Interfaces.FileExplorer;

/// <summary>
/// This interface is used for easy permission checks in <see cref="IUICFileExplorerService"/> that get converted to <see cref="IUICFileExplorerPermissionService"/>
/// <br>This interface exists for a easy implementation of the permissionchecks when creating a custom <see cref="IUICFileExplorerPermissionService"/></br>
/// </summary>
public interface IUICFileExplorerPermissionChecker
{
    /// <summary>
    /// Throws a exception if any of the sourcefiles or folders cannot be copied to the destinationpath. This includes checks for each subfile or subfolder
    /// </summary>
    /// <param name="sourceFiles">a list of files or folders that have to be copied</param>
    /// <param name="destinationPath">the destination path where the files are copied to</param>
    /// <returns></returns>
    public Task ThrowIfCurrentUserCantCopyFilesAsync(List<RelativePathModel> sourceFiles, RelativePathModel destinationPath);

    /// <summary>
    /// Throws a exception if any of the pathModels cannot be removed. This includes checks for each subfile or subfolder
    /// </summary>
    public Task ThrowIfCurrentUserCantDeleteFilesAsync(List<RelativePathModel> pathModels);

    /// <summary>
    /// Throws a exception if any of the sourcefiles or folders cannot be moved to the destinationpath. This includes checks for each subfile or subfolder
    /// </summary>
    /// <param name="sourceFiles">a list of files or folders that have to be moved</param>
    /// <param name="destinationPath">the destination path where the files are moved to</param>
    /// <returns></returns>
    public Task ThrowIfCurrentUserCantMoveFilesAsync(List<RelativePathModel> sourceFiles, RelativePathModel destinationPath);

    /// <summary>
    /// Throws a exception if this file does not exist or cannot be renamed by the current user.
    /// </summary>
    /// <param name="sourcePath"></param>
    /// <param name="newFilename"></param>
    /// <returns></returns>
    public Task ThrowIfCurrentUserCantRenameFileOrDirectory(RelativePathModel sourcePath, string newFilename);
    
}
