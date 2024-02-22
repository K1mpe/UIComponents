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

    public static string GetHtmlAttributes(this IUIHasAttributes component)
    {
        return component.Attributes.GetHtmlAttributes();
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
            if (action is IUIHasAttributes component)
            {
                component.AddAttribute(attribute, value);
            }
        }
    }

    public static void SetIdentifier(this IUIHasAttributes action, string identifier)
    {
        action.AddAttribute("identifier", identifier);
    }

    public static IHtmlContent IncludeScript(this IHtmlHelper html, Scripts script)
    {
        HtmlContentBuilder htmlContentBuilder = new HtmlContentBuilder();


        return htmlContentBuilder;
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
        var childComponents = uiComponent.GetAllChildren();
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

    public static void AssignCollectionForChildren<T>(this IUICScriptCollection collection, T uiComponent) where T : class, IUIHasAttributes, IUIComponent
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
        var styleString = styleTag.InnerHtml.RenderHtmlContent().Replace("<style>", "").Replace("</style", "");
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

    public static Task<IHtmlContent> RenderStylesAndScripts(this IUICScriptCollection collection, IViewComponentHelper component, IUIHasAttributes uiComponent)
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
    public static void AssignCollectionForChildren<T>(this T uiComponent) where T : class, IUIComponent, IUIHasAttributes, IUICHasScriptCollection
    {
        if (uiComponent.ScriptCollection == null)
            uiComponent.ScriptCollection = new UICScriptCollection();
        uiComponent.ScriptCollection.AssignCollectionForChildren(uiComponent);
    }

    public static Task<IHtmlContent> RenderStylesAndScripts<T>(this T uiComponent, IViewComponentHelper component) where T : IUIComponent, IUIHasAttributes, IUICHasScriptCollection
    {
        return uiComponent.ScriptCollection.RenderStylesAndScripts(component, uiComponent);
    }

    public static Task<IHtmlContent> RenderStylesAndScripts<T>(this T uiComponent, IViewComponentHelper component, string id) where T : IUIComponent, IUICHasScriptCollection
    {
        return uiComponent.ScriptCollection.RenderStylesAndScripts(component, id);
    }

    /// <summary>
    /// Add a script that will be rendered by the script collection. This scriptcollection is rendered inside a $(document).ready
    /// </summary>
    /// <param name="hasScriptCollection"></param>
    /// <param name="razerCode"></param>
    public static void AddScript(this IUICHasScriptCollection hasScriptCollection, RazerBlock razerCode)
    {
        hasScriptCollection.ScriptCollection.AddToScripts(new UICCustom(razerCode));
    }

    public static void AddScript(this IUICHasScriptCollection hasScriptCollection, IUIAction scriptComponent)
    {
        hasScriptCollection.ScriptCollection.AddToScripts(scriptComponent);
    }

    public static void AddStyle(this IUICHasScriptCollection hasScriptCollection, IUIComponent styleComponent)
    {
        hasScriptCollection.ScriptCollection.AddToStyles(styleComponent);
    }
    public static void AddStyle(this IUICHasScriptCollection hasScriptCollection, RazerBlock razerCode)
    {
        hasScriptCollection.ScriptCollection.AddToStyles(new UICCustom(razerCode));
    }
    #endregion

}
public enum Scripts
{
    UIC,
    Modal,
    GetPost,
}