using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;

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

    public static IHtmlContent JsEncode(this IHtmlHelper htmlHelper, string text, string brackets = null)
    {
        HtmlContentBuilder content = new();
        if (brackets != null)
            content.AppendHtml(htmlHelper.Raw(brackets));

        content.Append(Encode(text, null));

        if (brackets != null)
            content.AppendHtml(htmlHelper.Raw(brackets));

        return content;
    }

    public static async Task<IHtmlContent>Translate(this IHtmlHelper htmlHelper, UICLanguageService languageService, ITranslateable translateable, string brackets = "'")
    {
        var translated = await languageService.Translate(translateable);
        return htmlHelper.JsEncode(translated, brackets);
    }
}
