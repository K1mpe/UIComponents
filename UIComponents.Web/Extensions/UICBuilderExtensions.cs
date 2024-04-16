using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
using System.Text;
using UIComponents.Generators.Configuration;
using UIComponents.Generators.Registrations;
using UIComponents.Web.Components;
using UIComponents.Web.ModelBinders;

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

        var executingAssembly = Assembly.GetCallingAssembly();
        var currentAssembly = Assembly.GetExecutingAssembly();
        var currentVersion = currentAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
        string root = executingAssembly.Location;
        var manifestNames = currentAssembly.GetManifestResourceNames();
        string currentAssemblyName = currentAssembly.GetName().Name;
        var dir = Directory.GetCurrentDirectory();


        var versionFile = $"{dir}\\UIComponents\\Version.md";
        if (options.OnlyReplaceNewerVersion)
        {
            if (File.Exists(versionFile))
            {
                var content = File.ReadAllText(versionFile);
                if (content.StartsWith(currentVersion))
                    return services;
            }
        }

        if (options.ReplaceScripts)
        {
            string targetRoote = $"{dir}\\wwwroot\\uic\\js";
            string sourceRoute = $"{currentAssemblyName}.UIComponents.Root.";
            if (!Directory.Exists(targetRoote))
            {
                Directory.CreateDirectory(targetRoote);
            }

            var scripts = manifestNames.Where(x => x.EndsWith(".js")).OrderBy(x => x);


            var jsDestination = $"{targetRoote}\\uic.js";
            if (File.Exists(jsDestination))
                File.Delete(jsDestination);

            using (var jsFile = File.Create(jsDestination))
            {

                foreach (var script in scripts)
                {
                    using (var resourceStream = currentAssembly.GetManifestResourceStream(script))
                    {
                        resourceStream!.CopyTo(jsFile);
                    }
                }
            }

        }
        if (options.ReplaceCss)
        {

            string targetRoote = $"{dir}\\wwwroot\\uic\\css";
            string sourceRoute = $"{currentAssemblyName}.UIComponents.Root.";
            if (!Directory.Exists(targetRoote))
            {
                Directory.CreateDirectory(targetRoote);
            }

            var styles = manifestNames.Where(x => x.EndsWith(".css") || x.EndsWith(".scss"));


            var cssDestination = $"{targetRoote}\\uic.scss";
            if (File.Exists(cssDestination))
                File.Delete(cssDestination);

            using (var cssFile = File.Create(cssDestination))
            {

                foreach (var style in styles)
                {
                    using (var resourceStream = currentAssembly.GetManifestResourceStream(style))
                    {
                        resourceStream!.CopyTo(cssFile);
                    }
                }
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
                if (File.Exists(componentDir))
                    File.Delete(componentDir);

                using (var componentFile = File.Create(componentDir))
                {
                    using (var resourceStream = currentAssembly.GetManifestResourceStream(component))
                    {
                        resourceStream!.CopyTo(componentFile);
                    }
                }
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
                if (File.Exists(componentDir))
                    File.Delete(componentDir);

                using (var componentFile = File.Create(componentDir))
                {
                    using (var resourceStream = currentAssembly.GetManifestResourceStream(component))
                    {
                        resourceStream!.CopyTo(componentFile);
                    }
                }
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
                        resourceStream!.CopyTo(readMeFile);
                    }
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
