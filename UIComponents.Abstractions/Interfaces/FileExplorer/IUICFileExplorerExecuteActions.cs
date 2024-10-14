namespace UIComponents.Abstractions.Interfaces.FileExplorer;

public interface IUICFileExplorerExecuteActions
{
    public Task CreateDirectoryAsync(string path);
    public Task CopyFileAsync(string sourceFile, string destinationFile);
    public Task DeleteFileAsync(string filepath);
    public Task MoveFileAsync(string sourceFile, string destinationFile);
    public Task RenameFileAsync(string sourceFile, string newName);
    public Task RenameDirectoryAsync(string sourceFile, string newName);
}
