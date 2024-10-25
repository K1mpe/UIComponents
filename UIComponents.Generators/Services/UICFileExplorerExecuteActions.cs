using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces.FileExplorer;

namespace UIComponents.Generators.Services
{
    public class UICFileExplorerExecuteActions : IUICFileExplorerExecuteActions
    {
        private readonly ILogger _logger;

        public UICFileExplorerExecuteActions(ILogger<UICFileExplorerExecuteActions> logger)
        {
            _logger = logger;
        }

        public virtual Task CopyFileAsync(string sourceFile, string destinationFile)
        {
            return _logger.LogFunction($"Copying {sourceFile}", true, async () =>
            {
                using (var source = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
                {
                    if(File.Exists(destinationFile))
                        await DeleteFileAsync(destinationFile);

                    using (var target = File.Create(destinationFile))
                    {
                        await source.CopyToAsync(target);
                    }
                }
            }, LogLevel.Information);
        }

        public virtual Task CreateDirectoryAsync(string path)
        {
            _logger.LogInformation("Creating directory {0}", path);
            Directory.CreateDirectory(path);
            return Task.CompletedTask;
        }

        public virtual Task DeleteFileAsync(string filepath)
        {
            if(Directory.Exists(filepath))
            {
                var filesInDirectory = Directory.GetFiles(filepath, "*", SearchOption.AllDirectories);
                if (filesInDirectory.Any())
                    _logger.LogWarning("Deleting folder {0} while containing {1} files", filepath, filesInDirectory.Count());
                else
                    _logger.LogInformation("Deleting empty folder {0}", filepath);
                
                Directory.Delete(filepath, true);
                return Task.CompletedTask;
            }

            _logger.LogWarning("Deleting file {0}", filepath);
            File.Delete(filepath);
            return Task.CompletedTask;
        }

        public virtual Task MoveFileAsync(string sourceFile, string destinationFile)
        {
            return _logger.LogFunction("Moving {0}", true, async () =>
            {
                if (File.Exists(sourceFile))
                {
                    File.Move(sourceFile, destinationFile);
                } else if(Directory.Exists(destinationFile))
                {
                    Directory.Move(destinationFile, sourceFile);
                }
            }, LogLevel.Information);
        }

        public virtual Task RenameFileAsync(string sourceFile, string newName)
        {
            var directory = Path.GetDirectoryName(sourceFile);
            var targetFile = Path.Combine(directory, newName);
            
            _logger.LogInformation("Renaming file {0} to {1}", sourceFile, newName);
            File.Move(sourceFile, targetFile);
            return Task.CompletedTask;
        }
        public virtual Task RenameDirectoryAsync(string sourceDirectory, string newName)
        {
            var directory = new DirectoryInfo(sourceDirectory);
            var targetFile = Path.Combine(directory.Parent.FullName, newName);
            _logger.LogInformation("Renaming directory {0} to {1}", sourceDirectory, newName);
            Directory.Move(sourceDirectory, targetFile);
            return Task.CompletedTask;
        }

        public async Task AddFile(string filepath, Stream stream)
        {
            if(File.Exists(filepath))
                await DeleteFileAsync(filepath);

            using (var filestream = new FileStream(filepath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(filestream);
            }
        }
    }
}
