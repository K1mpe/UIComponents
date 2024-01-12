using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using UIComponents.Generators.Configuration;
using UIComponents.Generators.Registrations;
using UIComponents.Web.Components;

namespace UIComponents.Web.Extensions
{
    public static class UIComponentsServiceCollectionExtensions
    {

        public static IServiceCollection AddWebComponents(this IServiceCollection services, Action<UICConfigOptions> config)
        {
            services.AddMvcCore().AddRazorRuntimeCompilation(options =>
            {
                options.FileProviders.Add(new EmbeddedFileProvider(typeof(UICViewComponent)
                    .GetTypeInfo().Assembly));
            });

            services.AddUIComponent(config);

            return services;
        }

    }
}
