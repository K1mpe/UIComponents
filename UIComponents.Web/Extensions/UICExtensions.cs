using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;
using UIComponents.Abstractions.Varia;
using UIComponents.Web.Components;
using TagBuilder = Microsoft.AspNetCore.Mvc.Rendering.TagBuilder;

namespace UIComponents.Web.Extensions;

public static class UICExtensions
{
    /// <summary>
    /// Render the current component
    /// </summary>
    /// <param name="UIC"></param>
    /// <param name="component">Component should be available in cshtml</param>
    /// <returns></returns>
    public static async Task<Microsoft.AspNetCore.Html.IHtmlContent> InvokeAsync(this IUIComponent? UIC, IViewComponentHelper component)
    {
        if (UIC == null)
            return null;
        return await component.InvokeAsync(typeof(UICViewComponent), UIC);
    }
    public static async Task<Microsoft.AspNetCore.Html.IHtmlContent> InvokeAsync<T>(this Task<T> UIC, IViewComponentHelper component) where T : IUIComponent
    {
        var result = await UIC;
        if (result == null)
            return null;
        return await component.InvokeAsync(typeof(UICViewComponent), result);
    }

    public static async Task<Microsoft.AspNetCore.Html.IHtmlContent> InvokeAsync<T>(this IEnumerable<T> UIC, IViewComponentHelper component) where T : IUIComponent
    {
        var content = new HtmlContentBuilder();
        if(UIC == null)
            return content;
        foreach (var uic in UIC)
        {
            content.AppendHtml(await uic.InvokeAsync(component));
        }
        return content;
    }
    public static async Task<Microsoft.AspNetCore.Html.IHtmlContent> InvokeAsync<T>(this Task<List<T>> UIC, IViewComponentHelper component) where T : IUIComponent
    {
        var result = await UIC;
        if (result == null)
            return null;
        return await result.InvokeAsync(component);
    }
    public static Task<IHtmlContent> InvokeAsync(this IViewComponentHelper component, params IUIComponent[] UIC) => UIC.InvokeAsync(component);
    public static Task<IHtmlContent> InvokeAsync(this IViewComponentHelper component, IEnumerable<IUIComponent> UIC) => UIC.InvokeAsync(component);


    public static string GetHtmlAttributes(this Dictionary<string, string> dictionary)
    {
        string allAttributes = "";
        List<string> blackListAttributes = new()
    {
        "identifier",
        "formidentifier"
    };
        foreach (var attr in dictionary)
        {
            if (blackListAttributes.Contains(attr.Key.ToLower()))
                continue;

            allAttributes = string.Join(" ", allAttributes, $"{attr.Key.ToLower()}=\"{attr.Value}\"");
        }
        return allAttributes;
    }

    public static string GetHtmlAttributes(this IUICHasAttributes component)
    {
        return component.Attributes.GetHtmlAttributes();
    }

    public static IHtmlContent GetAttributesFromDictionary(this IHtmlHelper htmlHelper, Dictionary<string, string> dictionary)
    {
        string attributes = string.Empty;
        foreach(var attr in dictionary)
        {
            attributes = string.Join(" ", attributes, $"{attr.Key.ToLower()}=\"{attr.Value}\"");
        }
        return htmlHelper.Raw(attributes);
    }




    /// <summary>
    /// For each <see cref="IUIComponent"/> that inherents from the <see cref="UIComponent"/> class, add this attribute
    /// </summary>
    /// <param name="action"></param>
    /// <param name="attribute"></param>
    /// <param name="value"></param>
    public static void AddAttribute(this IEnumerable<IUIComponent> actions, string attribute, string value)
    {
        foreach (var action in actions)
        {
            if (action is IUICHasAttributes component)
            {
                component.AddAttribute(attribute, value);
            }
        }
    }

    public static void SetIdentifier(this IUICHasAttributes action, string identifier)
    {
        action.AddAttribute("identifier", identifier);
    }

    public static IHtmlContent IncludeScript(this IHtmlHelper html, Scripts script)
    {
        HtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();


        return htmlContentBuilder;
    }


    public static async Task<IHtmlContent> ConvertToJavascript(this Dictionary<string, object> dict, IUICLanguageService languageService, IHtmlHelper htmlHelper, IJsonHelper jsonHelper,  IViewComponentHelper componentHelper)
    {
        var collection = new HtmlContentBuilder();
        foreach(var item in dict)
        {
            IHtmlContent content = null;
            if (item.Value is IUIComponent component)
                content = await component.InvokeAsync(componentHelper);
            else if (item.Value is Translatable translatable)
                content = await htmlHelper.TranslateJs(languageService, translatable, "'");
            else 
                content = jsonHelper.Serialize(item.Value);
            collection.AppendHtml($"'{item.Key}': ");
            collection.AppendHtml(content);
            collection.Append(",");
            collection.AppendLine();
        }
        return collection;
    }

    #region ScriptCollection


    public static void AssignCollectionForChildren(this IUICScriptCollection collection, IUIComponent uiComponent, string id)
    {
        if (uiComponent is IUICScriptCollection)
            throw new ArgumentException($"IUIcScriptCollection cannot be used as uiComponent");

        if (collection == null)
            throw new ArgumentNullException(nameof(collection));

        //RenderParent is already assigned
        if (!string.IsNullOrEmpty(collection.RenderParentId))
            return;


        collection.RenderParentId = id;

        var childCollections = new List<IUICScriptCollection>();
        var childComponents = uiComponent.GetAllChildren().Select(x => x.Component);
        foreach (var childComponent in childComponents)
        {
            if (childComponent is IUICHasScriptCollection hasCollection)
            {

                if (hasCollection.ScriptCollection == null)
                    hasCollection.ScriptCollection = collection;
                else if (string.IsNullOrEmpty(hasCollection.ScriptCollection.RenderParentId))
                {
                    hasCollection.ScriptCollection.MergeIntoOtherColllection(ref collection);
                    hasCollection.ScriptCollection = collection;
                }
            }
        }



    }

    public static void AssignCollectionForChildren<T>(this IUICScriptCollection collection, T uiComponent) where T : class, IUICHasAttributes, IUIComponent
    {
        collection.AssignCollectionForChildren(uiComponent, uiComponent.GetId());
    }

    public static async Task<IHtmlContent> RenderStylesAndScripts(this IUICScriptCollection collection, IViewComponentHelper component, string id)
    {
        var styles = collection.GetStyles(id);
        var scripts = collection.GetScripts(id);



        var styleTag = new TagBuilder("style");
        var scriptTag = new TagBuilder("script");

        foreach (var style in styles)
        {
            styleTag.InnerHtml.AppendHtml(await style.InvokeAsync(component));
        }
        var styleString = styleTag.InnerHtml.RenderHtmlContent().Replace("<style>", "").Replace("</style>", "");
        styleTag.InnerHtml.Clear();
        styleTag.InnerHtml.AppendHtml(styleString);

        if (scripts.Any())
        {
            scriptTag.InnerHtml.Append("$(document).ready(function(){");
            foreach (var script in scripts)
            {
                scriptTag.InnerHtml.Append("{");
                scriptTag.InnerHtml.AppendHtml(await script.InvokeAsync(component));
                scriptTag.InnerHtml.Append("}");
            }
            scriptTag.InnerHtml.Append("});");

            var scriptString = scriptTag.InnerHtml.RenderHtmlContent().Replace("<script>", "").Replace("</script>", "");
            scriptTag.InnerHtml.Clear();
            scriptTag.InnerHtml.AppendHtml(scriptString);
        }


        HtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();
        if (styleTag.HasInnerHtml)
            htmlContentBuilder.AppendHtml(styleTag.RenderHtmlContent());

        if (scriptTag.HasInnerHtml)
            htmlContentBuilder.AppendHtml(scriptTag.RenderHtmlContent());

        return htmlContentBuilder;
    }

    public static Task<IHtmlContent> RenderStylesAndScripts(this IUICScriptCollection collection, IViewComponentHelper component, IUICHasAttributes uiComponent)
    {
        return collection.RenderStylesAndScripts(component, uiComponent.GetId());
    }


    // IUIComponentHasScriptCollection
    public static void AssignCollectionForChildren<T>(this T uiComponent, string id) where T : class, IUICHasScriptCollection, IUIComponent
    {
        if (uiComponent.ScriptCollection == null)
            uiComponent.ScriptCollection = new UICScriptCollection();
        uiComponent.ScriptCollection.AssignCollectionForChildren(uiComponent, id);
    }

    /// <summary>
    /// This is required when using the AddScripts or AddStyles methods, this will assign the current collection to all children, if this has not been assigned already
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="uiComponent"></param>
    public static void AssignCollectionForChildren<T>(this T uiComponent) where T : class, IUIComponent, IUICHasAttributes, IUICHasScriptCollection
    {
        if (uiComponent.ScriptCollection == null)
            uiComponent.ScriptCollection = new UICScriptCollection();
        uiComponent.ScriptCollection.AssignCollectionForChildren(uiComponent);
    }

    public static Task<IHtmlContent> RenderStylesAndScripts<T>(this T uiComponent, IViewComponentHelper component) where T : IUIComponent, IUICHasAttributes, IUICHasScriptCollection
    {
        return uiComponent.ScriptCollection.RenderStylesAndScripts(component, uiComponent);
    }

    public static Task<IHtmlContent> RenderStylesAndScripts<T>(this T uiComponent, IViewComponentHelper component, string id) where T : IUIComponent, IUICHasScriptCollection
    {
        return uiComponent.ScriptCollection.RenderStylesAndScripts(component, id);
    }

    #endregion

}
public enum Scripts
{
    UIC,
    Modal,
    GetPost,
}