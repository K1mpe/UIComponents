using Microsoft.AspNetCore.Http;
using UIComponents.Abstractions.Interfaces.Services;

namespace UIComponents.Web.Controllers;

public class UICController : Controller
{
    #region Fields

    private readonly IUICStoredEvents _storedEvents;
    private readonly IUICStoredComponents _components;

    #endregion

    #region Ctor
    public UICController(IUICStoredEvents storedEvents, IUICStoredComponents components)
    {
        _storedEvents = storedEvents;
        _components = components;
    }
    #endregion

    [HttpPost]
    public async Task<IActionResult> PostEvent(string key, Dictionary<string, string> values)
    {
        await _storedEvents.IncommingSignalRTrigger(key, values);
        return Json(true);
    }

    [HttpGet]
    public IActionResult GetComponent(string key)
    {
        var result = _components.GetComponent(key);
        
        if(IsAjaxReques(Request))
            return PartialView("/UIComponents/ComponentViews/Render.cshtml", result);
        return View("/UIComponents/ComponentViews/Render.cshtml", result);
    }

    public bool IsAjaxReques(HttpRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.Headers != null)
            return request.Headers["X-Requested-With"] == "XMLHttpRequest";

        return false;
    }
}
