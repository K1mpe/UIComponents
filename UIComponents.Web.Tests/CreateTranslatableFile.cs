using System.Reflection;
using System.Text.RegularExpressions;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Abstractions.Varia;

namespace UIComponents.Web.Tests
{
    public static class TranslatableFile
    {
        public static async void CreateTranslatableFile(this IApplicationBuilder app)
        {
            try
            {
                var results = TranslatableSaver.ScanSolution();
                var dir = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName;
                await TranslatableSaver.SaveToFileAsync(results, $"{dir}\\UIComponents.Web\\UIComponents\\Translations.json", false, false);
            }
            catch (Exception ex)
            {
                var logger = app.ApplicationServices.GetService<ILogger<Program>>();
                logger.LogError(ex, ex.Message);
            }
            
        }
    }
}
