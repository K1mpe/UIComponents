using System.Drawing;
using UIComponents.Abstractions;
using UIComponents.Abstractions.Interfaces;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Registrations;
using UIComponents.Web.Extensions;
using UIComponents.Web.Tests.Models;
using UIComponents.Web.Tests.Services;
using UIComponents.Web.Tests;
using UIComponents.Web.Tests.Validators;
using UIComponents.Web.Tests.Factory;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);


builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
});

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddMvc();

// Add optimizer to minify CSS and JavaScript files at runtime.



builder.Services.AddUIComponentWeb(config =>
{
    config.OnlyReplaceNewerVersion = false;
    config.CheckPermissionServiceType = true;
    config.CheckLanguageServiceType = false;
    config.CheckPropertyValidatorReadonly = false;
    config.AddChangeLog = true;
    config.AddReadMe = true;
    config.AddTranslationFile = true;
    config.AddFileExplorerImgs = true;
    config.AddDefaultGenerators(builder.Services);
    config.AddDefaultValidators(builder.Services);
    config.AddValidatorPropertyMinLength((propinfo, obj) =>
    {
        var minLengthAttr = propinfo.GetInheritAttribute<MinLengthAttribute>();
        if (minLengthAttr == null)
            return Task.FromResult<int?>(null);
        return Task.FromResult<int?>(minLengthAttr.Length);
    });
});
Console.WriteLine("");
Console.WriteLine("-- Components are generated --");
Console.WriteLine("");
builder.Services.AddSingleton<IUICPermissionService, PermissionService>();
builder.Services.AddSignalR(options =>
                {
                    options.EnableDetailedErrors = true;
                })
                .AddJsonProtocol(options =>
                {
                    // options.PayloadSerializerSettings.ContractResolver = new DefaultContractResolver(); // .NET 2.1 (?)
                    options.PayloadSerializerOptions.PropertyNamingPolicy = null;                       // .NET 5.0
                });
builder.Services.AddSingleton<IUICSignalRService, SignalRService>();
builder.Services.AddSingleton<SignalRService>();
builder.Services.AddSingleton<MainHub>();
builder.Services.AddScoped<TestModelValidator>();
builder.Services.AddScoped<TestComponentFactory>();
builder.Services.AddSingleton<TestService>();

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
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

app.CreateTranslatableFile();
app.UseWebOptimizer();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthorization();

// SignalR
app.MapHub<MainHub>("/MainHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
