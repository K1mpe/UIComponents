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
    /// Required for adding web elements for UICComponents. This method already includes <see cref="UIComponents.Generators.Registrations.UICConfigure.AddUIComponent(IServiceCollection, Action{UicConfigOptions})"/>
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IServiceCollection AddUIComponentWeb(this IServiceCollection services, Action<UicConfigOptions> config)
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
            Directory.CreateDirectory(targetRoote);
        }

        var scripts = manifestNames.Where(x => x.EndsWith(".js")).OrderBy(x=>x);
        var styles = manifestNames.Where(x => x.EndsWith(".css") || x.EndsWith(".scss"));


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
