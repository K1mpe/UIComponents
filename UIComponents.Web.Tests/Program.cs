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
builder.Services.AddSingleton<ILanguageService, LanguageService>();

builder.Services.AddMvc();

builder.Services.AddWebComponents(config =>
{
    config.SetLanguageService(typeof(LanguageService));
    config.AddDefaultGenerators(builder.Services);
});

var app = builder.Build();

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
