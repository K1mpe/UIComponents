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
using UIComponents.Abstractions.Interfaces.FileExplorer;
using System.Diagnostics;
using CeloxWortelen.DA.DataTypes;
using Microsoft.VisualBasic;
using UIComponents.Generators.Models.Arguments;
using UIComponents.Generators.Models;
using System;
using Newtonsoft.Json.Schema;
using System.Text.Json;

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
    UIComponents.Defaults.OptionDefaults.ToolbarPosition = UIComponents.Generators.Models.ToolbarPosition.AboveForm;
    UIComponents.Defaults.Models.UICUpload.DropzoneCss = "~/lib/dropzone/dropzone.min.css";
    UIComponents.Defaults.Models.UICUpload.DropzoneScript = "/lib/dropzone/dropzone.min.js";
    UIComponents.Defaults.Models.Graphs.TimeLineGraphDefault.Script_ChartJs_Plugin_Zoom = "/lib/chartjs-plugin-zoom/chartjs-plugin-zoom.js";
    config.AddUpdateMonitor((filepath, newFileStream, overwriteFile) =>
    {
        //Debugger.Break();
        overwriteFile();
    });
    config.AddDefaultGenerators(builder.Services);
    config.AddDefaultValidators(builder.Services);

    config.AddGenerator(GeneratorHelper.PropertyGenerator("PropWatcherInput", 999, async (args, existing) =>
    {
        if (existing != null)
            return GeneratorHelper.Next();

        if (args.CallCollection.CurrentCallType != UIComponents.Generators.Models.UICGeneratorPropertyCallType.PropertyInput)
            return GeneratorHelper.Next();

        if (args.PropertyValue == null)
            return GeneratorHelper.Next();

        if (!args.PropertyValue.GetType()?.IsAssignableTo(typeof(PropWatcher)) ?? true)
            return GeneratorHelper.Next();

        var value = args.PropertyValue as PropWatcher;

        var valueType = value.ValueType;

        var valuePropInfo = value.GetType().GetProperties().Where(x=>x.Name == (nameof(PropWatcher.Value))).First();

        var uicPropertyType = await args.Configuration.GetPropertyTypeAsync(valuePropInfo, args.Options);
        var cc = new UICCallCollection(UICGeneratorPropertyCallType.PropertyInput, args.CallCollection.Caller, args.CallCollection);
        var newArgs = new UICPropertyArgs(args.ClassObject, valuePropInfo, uicPropertyType, args.Options, cc, args.Configuration)
            .SetPropertyType(valueType)
            .SetPropertyValue(value.Value);
        var input = await args.Configuration.GetGeneratedResultAsync<UICPropertyArgs, IUIComponent, UICInput>($"Input for Propwatcher", newArgs, args.Options);
        if (input == null)
            return GeneratorHelper.Next();

        var serverResponse = new UICActionServerResponse(false, (args) =>
        {
            var value = JsonSerializer.Deserialize(args["Value"], valueType);
        });
        serverResponse.GetVariableData = new UICCustom($"{{Value: uic.getValue($('#{input.GetId()}'))}};");
        input.Actions.AfterChange = serverResponse;

        var eventListener = UICEvent<PropChangedEventArgs>.Create(handler => value.AfterValueChanged += handler, handler => value.AfterValueChanged -= handler);
        eventListener.Action = new UICCustom($"debugger; uic.setValue('#{input.GetId()}', args.NewValue);");
        input.AddScriptDocReady(eventListener);
        return GeneratorHelper.Success(input, true);
    }));
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
builder.Services.AddSingleton<IUICSignalRHub, MainHub>(provider => provider.GetService<MainHub>());
builder.Services.AddScoped<TestModelValidator>();
builder.Services.AddScoped<TestComponentFactory>();
builder.Services.AddSingleton<TestService>();
builder.Services.AddScoped<IUICFileExplorerPermissionService, FilePermissionService>();
builder.Services.AddSingleton<IUICGetCurrentUserId, GetCurrentUserService>();

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
