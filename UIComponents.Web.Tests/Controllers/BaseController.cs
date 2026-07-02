using Microsoft.AspNetCore.Mvc;

namespace UIComponents.Web.Tests.Controllers;

public abstract class BaseController : Controller
{

    protected IActionResult ViewOrPartial(IUIComponent component)
    {
        if (IsAjaxRequest(Request))
            return PartialView("ComponentRender", component);
        return View("ComponentRender", component);
    }

    protected static bool IsAjaxRequest(HttpRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.Headers != null)
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";

        return false;
    }
}
