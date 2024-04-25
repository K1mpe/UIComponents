
using Microsoft.AspNetCore.Mvc;
using UIComponents.Web.Tests.Models;

namespace UIComponents.Web.Tests.Controllers;

public class TableController : Controller
{
    private readonly IUIComponentGenerator generator;

    public TableController(IUIComponentGenerator generator)
    {
        this.generator = generator;
    }

    public IActionResult Index() => View();


    public async Task<IActionResult> Details(string testString)
    {
        var vm = new TestModel()
        {
            TestString = testString
        };
        var component = await generator.CreateComponentAsync(vm);
        return ViewOrPartial(component);
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
