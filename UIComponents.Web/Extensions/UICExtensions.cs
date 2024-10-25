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
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dict"></param>
    /// <param name="func">Takes the key and value as string, returns a string result</param>
    /// <returns></returns>
    public static IHtmlContent FlattenDictionary(this Dictionary<string, object> dict, Func<string, string,string> func= null)
    {
        if (func == null)
            func = (key, value) => $"{key}: {value},";
        var content = new HtmlContentBuilder();
        foreach (var kvp in dict)
        {
            content.AppendHtmlLine(Flatten(kvp.Key, kvp.Value));
        }
        return content;
        string Flatten(string key, object value)
        {
            
            if(value is not null && value is not string)
            {
                var subProperties = value.GetType().GetProperties();
                if (subProperties.Any())
                {
                    string content = string.Empty;
                    foreach(var subProp in subProperties)
                    {
                        var subValue = subProp.GetValue(value);
                        content = string.Join(Environment.NewLine, content, Flatten($"{key}.{subProp.Name}", subValue));
                    }
                    return content;
                }
            }
            return func(key, value?.ToString() ?? string.Empty);
            
        }
    }
    public static async Task<IHtmlContent> ConvertToJavascript(this Dictionary<string, object> dict, IUICLanguageService languageService, IHtmlHelper htmlHelper, IJsonHelper jsonHelper,  IViewComponentHelper componentHelper, bool splitObjectProperties = false)
    {
        var collection = new HtmlContentBuilder();
        foreach(var item in dict)
        {
            IHtmlContent content = null;
            if (item.Value is IUIComponent component)
                content = await component.InvokeAsync(componentHelper);
            else if (item.Value is Translatable translatable)
                content = await htmlHelper.TranslateJs(languageService, translatable, "'");
            else if(splitObjectProperties && item.Value != null && item.Value is not string)
            {
                var properties = item.Value.GetType().GetProperties();
                if (properties.Any())
                {
                    foreach(var prop in properties)
                    {
                        await FlattenObjectProperties(collection, prop, item.Key, item.Value, languageService, htmlHelper, jsonHelper, componentHelper);
                    }
                    continue;
                }
            }
            else 
                content = jsonHelper.Serialize(item.Value);
            collection.AppendHtml($"'{item.Key}': ");
            collection.AppendHtml(content);
            collection.Append(",");
            collection.AppendLine();
        }
        return collection;
    }
    private static async Task FlattenObjectProperties(
    HtmlContentBuilder collection,
    PropertyInfo property,
    string parentKey,
    object parentObject,
    IUICLanguageService languageService,
    IHtmlHelper htmlHelper,
    IJsonHelper jsonHelper,
    IViewComponentHelper componentHelper)
    {
        var propertyName = property.Name;
        var fullKey = $"{parentKey}.{propertyName}";

        // Get the property value
        var propertyValue = property.GetValue(parentObject);

        IHtmlContent content = null;

        if (propertyValue is IUIComponent component)
        {
            // Handle UI components
            content = await component.InvokeAsync(componentHelper);
        }
        else if (propertyValue is Translatable translatable)
        {
            // Handle translatable strings
            content = await htmlHelper.TranslateJs(languageService, translatable, "'");
        }
        else if (propertyValue != null && propertyValue is not string &&  propertyValue.GetType().GetProperties().Any())
        {
            // If property is an object, recurse into it
            foreach (var nestedProp in propertyValue.GetType().GetProperties())
            {
                await FlattenObjectProperties(collection, nestedProp, fullKey, propertyValue, languageService, htmlHelper, jsonHelper, componentHelper);
            }
            return;
        }
        else
        {
            // Serialize primitive or simple types
            content = jsonHelper.Serialize(propertyValue);
        }

        // Append the flattened property to the collection
        collection.AppendHtml($"'{fullKey}': ");
        collection.AppendHtml(content);
        collection.Append(",");
        collection.AppendLine();
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
            var styleContent = await style.InvokeAsync(component);
            if(!string.IsNullOrWhiteSpace(styleContent.RenderHtmlContent()))
                styleTag.InnerHtml.AppendHtml(styleContent);
        }
        var styleString = styleTag.InnerHtml.RenderHtmlContent().Replace("<style>", "").Replace("</style>", "");
        styleTag.InnerHtml.Clear();
        styleTag.InnerHtml.AppendHtml(styleString);

        if (scripts.Any())
        {
            List<IHtmlContent> scriptContents = new();
            foreach(var script in scripts)
            {
                var scriptContent = await script.InvokeAsync(component);
                if (!string.IsNullOrWhiteSpace(scriptContent.RenderHtmlContent().Replace("<script>", "").Replace("</script>", "")))
                    scriptContents.Add(scriptContent);
            }

            if (scriptContents.Any())
            {
                scriptTag.InnerHtml.Append("$(document).ready(function(){");
                foreach (var script in scriptContents)
                {
                    scriptTag.InnerHtml.Append("{");
                    scriptTag.InnerHtml.AppendHtml(script);
                    scriptTag.InnerHtml.Append("}");
                }
                scriptTag.InnerHtml.Append("});");
            }
            

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