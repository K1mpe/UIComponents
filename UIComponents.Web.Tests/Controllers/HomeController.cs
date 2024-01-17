using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UIComponents.Abstractions;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces;
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
            //return View();
            var testModel = new TestModel();
            var component = await _uic.CreateComponentAsync(testModel, new()
            {
                CheckboxRenderer = UIComponents.Models.Models.Inputs.CheckboxRenderer.ToggleSwitch,
                SelectlistSearableForItems = 2,
                ShowCardHeaders = true,
                StartInCard = new()
                {
                    MinWidth="400px",
                    MaxWidth="1000px"
                }
            });
            var toolbar = component.FindFirstChildOnType<UICButtonToolbar>();
            var card = component.FindFirstOnType<UICCard>();

            return View("ComponentRender", component);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}