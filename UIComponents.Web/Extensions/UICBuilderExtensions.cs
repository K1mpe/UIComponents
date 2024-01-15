using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using UIComponents.Generators.Configuration;
using UIComponents.Generators.Registrations;
using UIComponents.Web.Components;

namespace UIComponents.Web.Extensions;

public static class UICBuilderExtensions
{

    /// <summary>
    /// Required for adding web elements for UICComponents. This method already includes <see cref="UIComponents.Generators.Registrations.UICConfigure.AddUIComponent(IServiceCollection, Action{UICConfigOptions})"/>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IServiceCollection AddUIComponentWeb(this IServiceCollection services, Action<UICConfigOptions> config)
    {
        services.AddMvcCore()
        .AddRazorRuntimeCompilation(options =>
        {
            options.FileProviders.Add(new EmbeddedFileProvider(typeof(UICViewComponent)
                .GetTypeInfo().Assembly));
        });

        services.AddUIComponent(config);


        var executingAssembly = Assembly.GetCallingAssembly();
        var currentAssembly = Assembly.GetExecutingAssembly();

        string root = executingAssembly.Location;

        var manifestNames = currentAssembly.GetManifestResourceNames();
        string currentAssemblyName = currentAssembly.GetName().Name;
        var dir = Directory.GetCurrentDirectory();
        string targetRoote = $"{dir}\\wwwroot\\uic";
        string sourceRoute = $"{currentAssemblyName}.Root.";
        if (!Directory.Exists(targetRoote))
        {
            Directory.CreateDirectory(targetRoote+"\\js");
            Directory.CreateDirectory(targetRoote+"\\css");
        }
            
        foreach(var manifestName in manifestNames)
        {
            try
            {

                string targetFile = null;
                if (!manifestName.StartsWith(sourceRoute))
                    continue;
                string filename = manifestName.Substring(sourceRoute.Length);

                if (filename.StartsWith("js."))
                    targetFile = filename.Replace("js.", "js\\");
                else if (filename.StartsWith("css."))
                    targetFile = filename.Replace("css.", "css\\");
                else
                    targetFile = filename;


                string fullDestinationFile = $"{targetRoote}\\{targetFile}";
                if (File.Exists(fullDestinationFile))
                    File.Delete(fullDestinationFile);

                using (var resourceStream = currentAssembly.GetManifestResourceStream(manifestName))
                {
                    using (var fileStream = File.Create(fullDestinationFile))
                    {
                        resourceStream!.CopyTo(fileStream);
                    }
                }
            }
            catch(Exception ex) 
            {
                Console.WriteLine($"UIC Unable to write manifest file {manifestName}");
            }

        }

        return services;
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
