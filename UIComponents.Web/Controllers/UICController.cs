using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Generators.Helpers;
using UIComponents.Abstractions.Helpers;

namespace UIComponents.Web.Controllers;

public class UICController : Controller
{
    #region Fields

    private readonly IUICStoredEvents _storedEvents;
    private readonly IUICStoredComponents _components;
    private readonly ILogger _logger;
    #endregion

    #region Ctor
    public UICController(IUICStoredEvents storedEvents, IUICStoredComponents components, ILogger<UICController> logger)
    {
        _storedEvents = storedEvents;
        _components = components;
        _logger = logger;
    }
    #endregion

    [HttpPost]
    public async Task<IActionResult> PostEvent(string key, Dictionary<string, string> values)
    {
        try
        {
            await _storedEvents.IncommingSignalRTrigger(key, values);
            return Json(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to post event on key {key}");
            throw;
        }
        
    }

    [HttpGet]
    public IActionResult GetComponent(string key)
    {
        try
        {
            var result = _components.GetComponent(key);

            //var clone = InternalHelper.CloneObject(result, true, result.GetType());
            var clone = result;
            return ViewOrPartial(clone); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to get component on key {key}");
            throw;
        }
        
    }

    public IActionResult ViewOrPartial(IUIComponent component)
    {
        if (IsAjaxReques(Request))
            return PartialView("/UIComponents/ComponentViews/Render.cshtml", component);
        return View("/UIComponents/ComponentViews/Render.cshtml", component);
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
