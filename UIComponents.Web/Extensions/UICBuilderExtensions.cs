using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;
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

        string root = executingAssembly.Location;
        var manifestNames = currentAssembly.GetManifestResourceNames();
        string currentAssemblyName = currentAssembly.GetName().Name;
        var dir = Directory.GetCurrentDirectory();


        if (options.ReplaceScripts)
        {

            string targetRoote = $"{dir}\\wwwroot\\uic\\js";
            string sourceRoute = $"{currentAssemblyName}.Root.";
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
            string sourceRoute = $"{currentAssemblyName}.Root.";
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

            string targetRoote = $"{dir}\\UIComponents\\";
            string sourceRoute = $"{currentAssemblyName}.UIComponents.";
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
