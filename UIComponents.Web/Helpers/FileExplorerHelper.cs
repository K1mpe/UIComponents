using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Web.Helpers
{
    public static class FileExplorerHelper
    {
        public static async Task<IActionResult> DownloadFileOrZip(IEnumerable<string> files, HttpContext httpContext, ILogger? logger, LogLevel logLevel = LogLevel.Information, LogLevel loglevelZippedFiles = LogLevel.Debug, CompressionLevel zipCompressionLevel =CompressionLevel.NoCompression)
        {
            if (!files.Any())
                throw new ArgumentNullException(nameof(files));

            long size = 0;
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    var fileInfo = new FileInfo(file);
                    size += fileInfo.Length;
                    continue;
                }
                foreach (var subFile in Directory.GetFiles(file, "*", SearchOption.AllDirectories))
                {
                    var fileInfo = new FileInfo(subFile);
                    size += fileInfo.Length;
                }
            }
            httpContext.Response.Headers.Add("Estimated-Content-Length", size.ToString());

            string fileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.zip";

            // Determine the filename based on the provided files
            if (files.Count() == 1)
            {
                var fileInfo = new FileInfo(files.First());
                if (fileInfo.Exists)
                {
                    fileName = fileInfo.Name;  // Single file, use its name
                }
                else
                {
                    var dirInfo = new DirectoryInfo(files.First());
                    fileName = $"{dirInfo.Name}.zip";  // Single directory, use directory name + ".zip"
                }
            }

            // Set headers for the response
            httpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
            httpContext.Response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;

            if (files.Count() == 1)
            {
                var file = files.First();

                if (System.IO.File.Exists(file))
                {
                    var fileInfo = new FileInfo(file);
                    httpContext.Response.Headers.Add("Content-Length", fileInfo.Length.ToString());
                    using (logger.BeginScopeKvp("FilePath", file))
                    {
                        await logger.LogFunction("Downloading file", true, async () =>
                        {
                            using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
                            {
                                // Directly copy the file stream to the output stream (the response)
                                await fileStream.CopyToAsync(httpContext.Response.Body);
                            }
                        }, logLevel);
                    }

                    // Flush the output stream to ensure the response is sent to the client
                    await httpContext.Response.Body.FlushAsync();
                    return new EmptyResult(); // Exit after sending the single file
                }
            }
            
            // Create the ZipArchive directly in the response stream
            await logger.LogFunction("Creating Zip file", true, async () =>
            {
                using (var archive = new ZipArchive(httpContext.Response.Body, ZipArchiveMode.Create, leaveOpen: true))
                {
                    foreach (var file in files)
                    {
                        if (File.Exists(file))
                        {
                            using(logger.BeginScopeKvp("FilePath", file))
                            {
                                await logger.LogFunction("Adding file to Zip", true, async () =>
                                {
                                    var fileInfo = new FileInfo(file);
                                    var fileEntry = archive.CreateEntry(fileInfo.Name, zipCompressionLevel);

                                    using (var entryStream = fileEntry.Open())
                                    using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
                                    {
                                        await fileStream.CopyToAsync(entryStream);  // Stream each file directly into the ZIP
                                    }
                                }, loglevelZippedFiles);
                            }
                        }
                        else if (Directory.Exists(file))
                        {
                            var dirInfo = new DirectoryInfo(file);
                            foreach (var filePath in Directory.GetFiles(file, "*", SearchOption.AllDirectories))
                            {
                                // Correcting the relative path
                                var relativePath = Path.Combine(dirInfo.Name, Path.GetRelativePath(file, filePath));
                                var fileEntry = archive.CreateEntry(relativePath);
                                using (logger.BeginScopeKvp("FilePath", file))
                                {
                                    await logger.LogFunction("Adding file to Zip", true, async () =>
                                    {
                                        using (var entryStream = fileEntry.Open())
                                        {
                                            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
                                            {
                                                await fileStream.CopyToAsync(entryStream);  // Stream each file directly into the ZIP
                                            }
                                        }
                                    }, loglevelZippedFiles);
                                }
                            }
                        }
                    }
                }
            }, logLevel);

            // Flush the output stream to ensure all data is sent to the client
            await httpContext.Response.Body.FlushAsync();
            return new EmptyResult();
        }
    }
}
