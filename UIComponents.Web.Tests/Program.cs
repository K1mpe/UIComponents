using UIComponents.Abstractions;
using UIComponents.Abstractions.Interfaces;
using UIComponents.Abstractions.Interfaces.ExternalServices;
using UIComponents.Generators.Registrations;
using UIComponents.Web.Extensions;
using UIComponents.Web.Tests.Services;

var builder = WebApplication.CreateBuilder(args);


builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddMvc();

// Add optimizer to minify CSS and JavaScript files at runtime.


builder.Services.AddUIComponentWeb(config =>
{
    UIComponents.Defaults.OptionDefaults.ReverseButtonOrder = false;
    config.AddDefaultGenerators(builder.Services);
});

builder.Services.AddWebOptimizer(pipeline =>
{
    var inProduction = builder.Environment.IsProduction();
    if (inProduction || true)
    {
        pipeline.MinifyHtmlFiles();
        pipeline.MinifyCssFiles();
        //pipeline.MinifyJsFiles();

        // Bundle all files in the css folder and its subfolders.
        pipeline.AddScssBundle("/css/site.css", "/css/**/*");
    }

    var options = new WebOptimizer.Sass.WebOptimizerScssOptions { MinifyCss = inProduction };
    pipeline.CompileScssFiles(options);
});
var app = builder.Build();
app.UseWebOptimizer();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
