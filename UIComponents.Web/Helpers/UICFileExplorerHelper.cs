using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Web.Helpers
{
    public static class UICFileExplorerHelper
    {
        /// <summary>
        /// This method can stream a one or more files or folders to the client for download. If there is more than one file to be transfered, a zip is created and streamed to the client
        /// </summary>
        /// <remarks>
        /// Since this method uses streaming, there is no heavy memory usage on the server or client for large files.
        /// </remarks>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task<IActionResult> DownloadFileOrZipStream(IEnumerable<string> files, HttpContext httpContext, ILogger? logger, LogLevel logLevel = LogLevel.Information, LogLevel loglevelZippedFiles = LogLevel.Debug, CompressionLevel zipCompressionLevel = CompressionLevel.NoCompression)
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


        private static Dictionary<string, DateTime> StartUploadingFiles = new();

        /// <summary>
        /// This methods saves all files from the <see cref="HttpContent"/> and saves them in the target directory.
        /// <br>If Dropzone is used with chinking, This method can stream download the files</br>
        /// </summary>
        /// <remarks>
        /// All existing files with the same name will be removed before the upload! make sure to check all of them before calling this method!
        /// </remarks>
        /// <param name="httpContext"></param>
        /// <param name="targetDirectory"></param>
        /// <returns></returns>
        public static async Task UploadFilesFromDropzoneStream(HttpContext httpContext, string targetDirectory, ILogger logger = null)
        {
            var form = httpContext.Request.Form;
            if (form.ContainsKey("dzchunkindex"))
            {
                var chunkIndex = int.Parse(form["dzchunkindex"]);
                var totalChunks = int.Parse(form["dztotalchunkcount"]);
                var fileSize = long.Parse(form["dztotalfilesize"]);

                var file = form.Files.FirstOrDefault();
                if (file == null)
                    return;
                var filename = form["dzuuid"] + "_" + file.FileName;

                var filePath = Path.Combine(Path.GetTempPath(), filename);
                var finalFilePath = Path.Combine(targetDirectory, file.FileName);
                using (logger?.BeginScopeKvp(
                    new("FilePath", finalFilePath),
                    new("FileSize", fileSize)))
                { 
                    try
                    {
                        if (chunkIndex == 0 && logger != null)
                        {
                            logger.LogInformation("Start uploading file stream");
                            lock (StartUploadingFiles)
                            {
                                StartUploadingFiles[finalFilePath] = DateTime.Now;
                            }
                        }


                        // Append the current chunk to the file on the server
                        using (var stream = new FileStream(filePath, FileMode.Append))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Check if this is the last chunk, if so we can finalize the upload
                        if (chunkIndex == totalChunks-1)
                        {
                            // Optional: Move the file to the final destination, or perform any processing needed
                            if (File.Exists(finalFilePath))
                                File.Delete(finalFilePath);
                            System.IO.File.Move(filePath, finalFilePath);

                            if(StartUploadingFiles.TryGetValue(finalFilePath, out var dateTime))
                            {
                                var timePassed = DateTime.Now - dateTime;
                                var formatted = UIComponents.Defaults.FormatDefaults.FormatTimespan(timePassed);
                                logger?.LogInformation("Finished uploading file stream in {0}", formatted);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        logger?.LogInformation("Uploading file has been canceled");
                        lock (StartUploadingFiles)
                        {
                            StartUploadingFiles.Remove(filePath);
                        }
                        throw;
                    }
                    catch
                    {
                        logger?.LogError("Error while uploading file stream");
                        lock (StartUploadingFiles)
                        {
                            StartUploadingFiles.Remove(filePath);
                        }
                        throw;
                    }
                }

            }
            else
            {
                foreach (var file in form.Files)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    string filepath = Path.Combine(targetDirectory, fileName);
                    if (System.IO.File.Exists(filepath))
                        System.IO.File.Delete(filepath);
                    using (logger.BeginScopeKvp("FilePath", filepath))
                    {
                        await logger.LogFunction("Uploading file", true, async () =>
                        {
                            using (var fileStream = new FileStream(filepath, FileMode.Create, FileAccess.Write))
                            {
                                await file.CopyToAsync(fileStream);
                            }
                        }, LogLevel.Information);
                    }
                }
            }
        }
    }
}
