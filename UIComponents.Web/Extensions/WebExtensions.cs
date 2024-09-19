using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Drawing;
using System.Runtime.Serialization;

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

    /// <summary>
    /// <br>$('#id').on('uic-help, ()=>{</br>
    /// <br>    console.groupCollapsed( 'typename: $('#id')')</br>
    /// <br>    content('id')  [ id auto set if not found in content ]</br>
    /// <br>    console.groupEnd</br>
    /// <br>}</br>
    /// </summary>
    /// <param name="htmlHelper"></param>
    /// <param name="model"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static IHtmlContent UICHelp(this IHtmlHelper htmlHelper, UIComponent model, string content)
    {
        var contentString = content;
        if (!contentString.Contains(model.GetId()))
            contentString += $"('{model.GetId()}')";
        return htmlHelper.Raw($"\r\n$('#{model.GetId()}').on('uic-help', ()=>{{console.groupCollapsed('%c'+'{model.GetType().Name}: $(\"#{model.GetId()}\")', `color: ${{uic.consoleColor()}}`); {contentString}; console.groupEnd();}})\r\n");
    }
}
