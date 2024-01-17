using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace UIComponents.Web.Extensions;

public static class WebExtensions
{
    /// <summary>
    /// Parse a datetime to moment using ISO_8601
    /// </summary>
    /// <param name="dateTime"></param>
    /// <returns>
    /// moment('YYYY-MM-dd HH:mm:ss', moment.ISO_8601)
    /// </returns>
    public static IHtmlContent ToMoment(this DateTime dateTime, IHtmlHelper htmlHelper)
    {
        return htmlHelper.Raw($"moment(\"{dateTime.ToString("yyyy-MM-dd HH:mm:ss")}\", moment.ISO_8601)");
    }
}
