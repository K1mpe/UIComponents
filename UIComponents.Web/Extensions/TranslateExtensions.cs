using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net;
using System.Text.Encodings.Web;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Abstractions.Varia;
using static System.Net.Mime.MediaTypeNames;
using static UIComponents.Abstractions.Varia.TranslatableSaver;

namespace UIComponents.Web.Extensions;

public static class TranslateExtensions
{
    /// <summary>
    /// Encode a string that is safe to parse in javascript. 
    /// </summary>
    /// <param name="text"></param>
    /// <param name="brackets">if provided, surround the result with these brackets. f.e. " ' "</param>
    /// <returns></returns>
    public static string Encode(string text, char? brackets = null)
    {
        var result = JavaScriptEncoder.Default.Encode(text ?? "");
        if (brackets != null)
            return brackets + result + brackets;

        return result;

    }

    public static IHtmlContent JsEncode(this IHtmlHelper htmlHelper, string text, string? brackets = null)
    {
        HtmlContentBuilder content = new();
        if (brackets != null)
            content.AppendHtml(htmlHelper.Raw(brackets));

        content.Append(Encode(text, null));

        if (brackets != null)
            content.AppendHtml(htmlHelper.Raw(brackets));

        return content;
    }

    public static async Task<IHtmlContent>TranslateJs(this IHtmlHelper htmlHelper, IUICLanguageService languageService, Translatable translateable, string brackets = "'")
    {
        if (translateable == null)
            return htmlHelper.JsEncode(null, brackets);
        var translated = await languageService.Translate(translateable);
        return htmlHelper.JsEncode(translated, brackets);
    }

    public static async Task<IHtmlContent>TranslateHtml(this IHtmlHelper htmlHelper, IUICLanguageService languageService, Translatable translatable, string brackets = null)
    {
        HtmlContentBuilder content = new();
        if (brackets != null)
            content.AppendHtml(htmlHelper.Raw(brackets));

        content.AppendHtml(htmlHelper.Raw(await languageService.Translate(translatable)));

        if (brackets != null)
            content.AppendHtml(htmlHelper.Raw(brackets));

        return content;
    }

}
