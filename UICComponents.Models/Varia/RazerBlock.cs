using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace UIComponents.ComponentModels.Varia;

/// <summary>
/// In cshtml, use <example>`@<text>...</text>`</example> to write html or javascript in this object.
/// </summary>
public delegate IHtmlContent RazerBlock(object item);


public static class RazerBlockExtentions
{
    public static string GetContent(this RazerBlock block)
    {
        var result = block.Invoke(null);
        string stringResult = result.RenderHtmlContent()?.Trim();
        return stringResult;
    }

    public static string RenderHtmlContent(this IHtmlContent htmlContent)
    {
        using var writer = new StringWriter();
        htmlContent.WriteTo(writer, HtmlEncoder.Default);
        return writer.ToString();
    }
}