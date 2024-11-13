using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using UIComponents.Generators.Configuration;
using UIComponents.Generators.Registrations;
using UIComponents.Web.Components;
using UIComponents.Web.ModelBinders;
using static System.Net.Mime.MediaTypeNames;

namespace UIComponents.Web.Extensions;

public static class UICBuilderExtensions
{

    /// <summary>
    /// Required for adding web elements for UICComponents. This method already includes <see cref="UIComponents.Generators.Registrations.UICConfigure.AddUIComponent(IServiceCollection, Action{UicConfigOptions})"/>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IServiceCollection AddUIComponentWeb(this IServiceCollection services, Action<UicConfigOptions> config)
    {
        services.AddUIComponent(config, out var options);

        services.AddMvcCore(mvc =>
        {
            mvc.ModelBinderProviders.Insert(0, new RecurringDateModelBinderProvider());
        });

        var provider = services.BuildServiceProvider();
        var updatedFileReplacer = provider.GetService<IUICUpdateMonitor>();
        bool hasUpdatedFileReplacer = updatedFileReplacer != null;

        var executingAssembly = Assembly.GetCallingAssembly();
        var currentAssembly = Assembly.GetExecutingAssembly();
        var currentVersion = currentAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
        string root = executingAssembly.Location;
        var manifestNames = currentAssembly.GetManifestResourceNames();
        string currentAssemblyName = currentAssembly.GetName().Name;
        var dir = Directory.GetCurrentDirectory();


        var versionFile = $"{dir}\\UIComponents\\Version.md";
        bool newerVersionAvailable = true;
        if (File.Exists(versionFile))
        {
            var content = File.ReadAllText(versionFile);
            newerVersionAvailable = !content.StartsWith(currentVersion);
        }
        if (options.OnlyReplaceNewerVersion && !newerVersionAvailable)
        {
            return services;
        }

        if (options.ReplaceScripts)
        {
            string targetRoote = $"{dir}\\wwwroot\\uic\\js";
            string sourceRoute = $"{currentAssemblyName}.UIComponents.Root.js";
            if (!Directory.Exists(targetRoote))
            {
                Directory.CreateDirectory(targetRoote);
            }

            var scripts = manifestNames.Where(x => x.StartsWith(sourceRoute)).OrderBy(x => x);


            foreach (var script in scripts)
            {
                string scriptDir = FixFilePath(script.Replace(sourceRoute, targetRoote));
                var fileInfo = new FileInfo(scriptDir);
                Directory.CreateDirectory(fileInfo.DirectoryName);

                if (script.EndsWith(".js"))
                {
                    WriteNewFile(scriptDir, script);
                    
                } else if (script.EndsWith(".cshtml"))
                {
                    if (File.Exists(scriptDir))
                        File.Delete(scriptDir);
                    scriptDir = scriptDir.Substring(0, scriptDir.Length - 6) + "js";
                    using (var scriptFile = File.Create(scriptDir))
                    {
                        using (var resourceStream = currentAssembly.GetManifestResourceStream(script))
                        {
                            using (var streamReader = new StreamReader(resourceStream))
                            {
                                using (var streamWriter = new StreamWriter(scriptFile))
                                {
                                    string text = streamReader.ReadToEnd();
                                    text = text.Replace("<script>", string.Empty).Replace("</script>", string.Empty);
                                    text = Regex.Replace(text, @"@\bnameof\b\(([^)]*)\)", "$1"); //This regex will remove all @nameof( ) variables and keep the content
                                    streamWriter.WriteLine(text);
                                }
                            }
                        }
                    }
                }
            }


            #region ScriptCollectionFile
            try
            {
                string collection = $"{dir}\\Views\\Shared\\_Scripts.UIC.cshtml";
                string collectionContent = string.Empty;
                AddScript("uic.js");

                foreach (var script in scripts)
                {
                    var scriptName = script.Replace(sourceRoute + ".", string.Empty);

                    if (scriptName.EndsWith(".cshtml")) //replace the cshtml with js
                        scriptName = scriptName.Substring(0, scriptName.Length - 6) + "js";

                    AddScript(scriptName);
                }
                if (File.Exists(collection))
                    File.Delete(collection);
                using (var collectionFile = File.Create(collection))
                {
                    using (var sr = new StreamWriter(collectionFile))
                    {
                        sr.WriteLine(collectionContent);
                    }
                }

                void AddScript(string name)
                {
                    string addingScript = $"<script src=\"~/uic/js/{name}?v={currentVersion}\"></script>";
                    if (collectionContent.Contains(addingScript))
                        return;
                    collectionContent += addingScript;
                    collectionContent += Environment.NewLine;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to write Scripts file. Error can be ignored if _scripts.UIC.cshtml is already in ");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            #endregion
        }
        if (options.ReplaceCss)
        {

            string targetRoote = $"{dir}\\wwwroot\\uic\\css";
            string sourceRoute = $"{currentAssemblyName}.UIComponents.Root.css";
            

            var styles = manifestNames.Where(x => x.EndsWith(".css") || x.EndsWith(".scss"));


            foreach (var style in styles)
            {
                var cssDestination = FixFilePath(style.Replace(sourceRoute, targetRoote));
                if (cssDestination.EndsWith("_outside-dependencies.scss") && File.Exists(cssDestination))
                    continue;
                WriteNewFile(cssDestination, style);
            }
        }
        if (options.ReplaceComponents)
        {

            string targetRoote = $"{dir}\\UIComponents\\ComponentViews\\";
            string sourceRoute = $"{currentAssemblyName}.UIComponents.ComponentViews.";
            if (!Directory.Exists(targetRoote))
            {
                Directory.CreateDirectory(targetRoote);
            }

            var components = manifestNames.Where(x => x.StartsWith(sourceRoute));

            foreach(var component in components)
            {
                string componentDir = FixFilePath(component.Replace(sourceRoute, targetRoote));
                var fileInfo = new FileInfo(componentDir);
                Directory.CreateDirectory(fileInfo.DirectoryName);
                WriteNewFile(componentDir, component);
            }

            
        }
        if (options.ReplaceTaghelpers)
        {

            string targetRoote = $"{dir}\\UIComponents\\Taghelpers\\";
            string sourceRoute = $"{currentAssemblyName}.UIComponents.Taghelpers.";
            if (!Directory.Exists(targetRoote))
            {
                Directory.CreateDirectory(targetRoote);
            }

            var components = manifestNames.Where(x => x.StartsWith(sourceRoute));

            foreach (var component in components)
            {
                string componentDir = FixFilePath(component.Replace(sourceRoute, targetRoote));
                var fileInfo = new FileInfo(componentDir);
                Directory.CreateDirectory(fileInfo.DirectoryName);
                WriteNewFile(componentDir, component);
            }
        }
        if (options.AddReadMe)
        {
            string targetRoote = $"{dir}\\UIComponents\\";
            string sourceRoute = $"{currentAssemblyName}";
            if (!Directory.Exists(targetRoote))
            {
                Directory.CreateDirectory(targetRoote);
            }

            var readmes = manifestNames.Where(x => x.EndsWith("README.md")).OrderBy(x => x);


            var jsDestination = $"{targetRoote}\\README.md";
            if (File.Exists(jsDestination))
                File.Delete(jsDestination);

            using (var readMeFile = File.Create(jsDestination))
            {
                foreach (var readme in readmes)
                {
                    using (var resourceStream = currentAssembly.GetManifestResourceStream(readme))
                    {
                        resourceStream!.CopyTo(readMeFile);
                    }
                }
            }

        }
        if (options.AddChangeLog)
        {
            string targetRoote = $"{dir}\\UIComponents\\";
            string sourceRoute = $"{currentAssemblyName}.UIComponents.ChangeLog.";
            if (!Directory.Exists(targetRoote))
            {
                Directory.CreateDirectory(targetRoote);
            }

            var readmes = manifestNames.Where(x => x.Contains(sourceRoute) && x.EndsWith(".md")).OrderByDescending(x => x);


            var destination = $"{targetRoote}\\CHANGELOG.md";
            if (File.Exists(destination))
                File.Delete(destination);

            using (var readMeFile = File.Create(destination))
            {
                foreach (var readme in readmes)
                {
                    using (var resourceStream = currentAssembly.GetManifestResourceStream(readme))
                    {
                        //Start after the UTF-8 BOM
                        resourceStream.Position = 3;
                        resourceStream!.CopyTo(readMeFile);
                    }

                    var newLine = Encoding.ASCII.GetBytes(Environment.NewLine);
                    readMeFile.Write(newLine, 0, newLine.Length);
                    readMeFile.Write(newLine, 0, newLine.Length);
                }
            }

        }

        if (options.AddFileExplorerImgs)
        {
            string targetRoote = $"{dir}\\wwwroot\\uic\\img\\file-explorer";
            string sourceRoute = $"{currentAssemblyName}.UIComponents.Root.img.file_explorer";
            if (!Directory.Exists(targetRoote))
            {
                Directory.CreateDirectory(targetRoote);
            }

            var imgs = manifestNames.Where(x => x.StartsWith(sourceRoute));

            foreach (var img in imgs)
            {
                string componentDir = FixFilePath(img.Replace(sourceRoute, targetRoote));
                var fileInfo = new FileInfo(componentDir);
                Directory.CreateDirectory(fileInfo.DirectoryName);
                if (File.Exists(componentDir))
                    continue;

                using (var componentFile = File.Create(componentDir))
                {
                    using (var resourceStream = currentAssembly.GetManifestResourceStream(img))
                    {
                        resourceStream!.CopyTo(componentFile);
                    }
                }
            }
        }
        if (options.AddTranslationFile)
        {
            string targetRoote = $"{dir}\\UIComponents\\";
            string sourceRoute = $"{currentAssemblyName}";
            if (!Directory.Exists(targetRoote))
            {
                Directory.CreateDirectory(targetRoote);
            }

            var translations = manifestNames.Where(x => x.EndsWith("Translations.json")).OrderBy(x => x).FirstOrDefault();
            if (!string.IsNullOrEmpty(translations))
            {
                var dest = $"{targetRoote}\\Translations.json";
                if (File.Exists($"{targetRoote}\\Translations.xml"))
                    File.Delete(dest);

                WriteNewFile(dest, translations);
            }
        }

        #region CreateVersionFile
        var targetFile = $"{dir}\\UIComponents\\Version.md";
        if (File.Exists(targetFile))
            File.Delete(targetFile);
        using(var versionFileWriter = File.Create(targetFile))
        {
            string text = currentVersion;
            text += Environment.NewLine;
            text += Environment.NewLine;
            text += "If you remove this file, the UIComponents will rebuild the file on next startup.";
            text += Environment.NewLine;
            text += $"This file is used for UIConfigOptions.${nameof(UicConfigOptions.OnlyReplaceNewerVersion)} if the version number matches";
            text += Environment.NewLine;
            text += $"If this version matches the version of UIComponents, no components, scripts, etc will be created on build with UIConfigOptions.${nameof(UicConfigOptions.OnlyReplaceNewerVersion)} on.";
            text += Environment.NewLine;

            var bytes = new UTF8Encoding(true).GetBytes(text);
            versionFileWriter.Write(bytes, 0, bytes.Length);
        }
        #endregion

        return services;

        void WriteNewFile(string destination, string manifestFile)
        {
            var fileInfo = new FileInfo(destination);
            if (!Directory.Exists(fileInfo.DirectoryName))
                Directory.CreateDirectory(fileInfo.DirectoryName);
            using (var resourceStream = currentAssembly.GetManifestResourceStream(manifestFile))
            {
                if (File.Exists(destination) && hasUpdatedFileReplacer)
                {
                    bool streamsAreEqual = false;
                    using(var filestream = File.Open(destination, FileMode.Open, FileAccess.Read))
                    {
                        streamsAreEqual = StreamsAreEqual(filestream, resourceStream);
                        
                    }
                    if(!streamsAreEqual)
                        updatedFileReplacer.FileWillBeUpdated(destination, resourceStream, () => ExecuteWriteFile(resourceStream));
                }
                else
                {
                    ExecuteWriteFile(resourceStream);
                }
            }

            void ExecuteWriteFile(Stream resourceStream)
            {
                if (File.Exists(destination))
                    File.Delete(destination);


                using (var cssFile = File.Create(destination))
                {
                    resourceStream!.CopyTo(cssFile);
                }
            }
        }
    }

    public static bool StreamsAreEqual(Stream stream1, Stream stream2)
    {
        if (stream1.Length != stream2.Length)
            return false;

        const int bufferSize = 1024 * 8; // 8KB buffer
        byte[] buffer1 = new byte[bufferSize];
        byte[] buffer2 = new byte[bufferSize];

        while (true)
        {
            int bytesRead1 = stream1.Read(buffer1, 0, bufferSize);
            int bytesRead2 = stream2.Read(buffer2, 0, bufferSize);

            if (bytesRead1 != bytesRead2)
                return false;

            if (bytesRead1 == 0) // End of stream reached for both
                return true;

            // Compare buffers
            for (int i = 0; i < bytesRead1; i++)
            {
                if (buffer1[i] != buffer2[i])
                    return false;
            }
        }
        throw new NotImplementedException();
    }
    private static string FixFilePath(string targetPath)
    {
        string currentDirectory = Directory.GetCurrentDirectory();
        if (!targetPath.StartsWith(currentDirectory))
            throw new ArgumentException("File target does not start in current directory");

        targetPath = targetPath.Replace(currentDirectory, string.Empty);
        var parts = targetPath.Split('.');
        var result = currentDirectory;
        for (int i = 0; i < parts.Length; i++)
        {
            if (i == parts.Length - 1)
                result += ".";
            else
                result+=$"\\";
            result += parts[i];
        }
        return result;
    }

    public static IApplicationBuilder MapUIC(this IApplicationBuilder app, string localPath)
    {
        localPath = "/UIC";
        app.Map(localPath, builder =>
        {
            var provider = new ManifestEmbeddedFileProvider(
                assembly: Assembly.GetExecutingAssembly(), "Root");
            builder.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = provider
            });
        });

        return app;
    }
}
