using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using UIComponents.Abstractions;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces;
using UIComponents.Abstractions.Models;
using UIComponents.Generators.Interfaces;
using UIComponents.Models.Extensions;
using UIComponents.Models.Models;
using UIComponents.Models.Models.Actions;
using UIComponents.Models.Models.Buttons;
using UIComponents.Models.Models.Card;
using UIComponents.Models.Models.Inputs;
using UIComponents.Models.Models.Texts;
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
            var testModel = new TestModel();
            var component = await _uic.CreateComponentAsync(testModel, new()
            {
                CheckboxRenderer = UIComponents.Models.Models.Inputs.CheckboxRenderer.ToggleSwitch,
                SelectlistSearableForItems = 2,
                ShowCardHeaders = true,
                FormToolbarInCardFooter = true,
                PostForm= new UICActionGetPost(UICActionGetPost.ActionTypeEnum.Post, "Home", "Post", new {Test="blub"})
            });
            (component as UIComponent).AddAttribute("min-width", "400px").AddAttribute("max-width", "1000px");

            component.FindInputGroupByPropertyName(nameof(TestModel.Decimal)).AppendInput.Add(new UICSpan(new Untranslated("km")));

            await Task.Delay(5000);
            return ViewOrPartial(component);
        }

        public async Task<IActionResult> Test()
        {
            await Task.Delay(500);
            return PartialView();
        }

        [HttpPost]
        public IActionResult Post(TestModel post)
        {
            var x = Request;
            return Json(true);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult ViewOrPartial(IUIComponent component)
        {
            if (IsAjaxRequest(Request))
                return PartialView("ComponentRender", component);
            return View("ComponentRender", component);
        }

        public static bool IsAjaxRequest(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";

            return false;
        }
    }
}