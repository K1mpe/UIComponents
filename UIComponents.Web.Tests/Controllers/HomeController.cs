using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using UIComponents.Abstractions;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces;
using UIComponents.Abstractions.Models;
using UIComponents.Generators.Interfaces;
using UIComponents.Models.Models;
using UIComponents.Models.Models.Buttons;
using UIComponents.Models.Models.Card;
using UIComponents.Models.Models.Inputs;
using UIComponents.Web.Tests.Models;

namespace UIComponents.Web.Tests.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUIComponentService _uic;
        public HomeController(ILogger<HomeController> logger, IUIComponentService uic)
        {
            _logger = logger;
            _uic = uic;
        }

        public async Task<IActionResult> Index()
        {
            return View();
            //return View();
            var testModel = new TestModel();
            var component = await _uic.CreateComponentAsync(testModel, new()
            {
                CheckboxRenderer = UIComponents.Models.Models.Inputs.CheckboxRenderer.ToggleSwitch,
                SelectlistSearableForItems = 2,
                ShowCardHeaders = true,
                FormToolbarInCardFooter = true,
                
            });
            var toolbar = component.FindFirstChildOnType<UICButtonToolbar>();
            var card = component.FindFirstOnType<UICCard>();
            (component as UIComponent).AddAttribute("min-width", "400px").AddAttribute("max-width", "1000px");

            return View("ComponentRender", component);
        }

        public IActionResult Test()
        {
            return PartialView();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}