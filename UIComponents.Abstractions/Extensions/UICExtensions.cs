using System.Collections;
using System.Reflection;
using UIComponents.Abstractions.Models;

namespace UIComponents.Abstractions.Extensions;

public static class UICExtensions
{

    /// <summary>
    /// This function will check each property for IUICs, this works recursive untill all children are found
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static List<IUIComponent> GetAllChildren(this IUIComponent element)
    {
        var list = new List<IUIComponent>();
        if (element == null)
            return list;
        foreach (var prop in element.GetType().GetProperties())
        {

            if (prop.GetCustomAttribute<IgnoreGetChildrenFunctionAttribute>() != null)
                continue;

            if (prop.PropertyType.IsAssignableTo(typeof(IUIComponent)))
            {

                var value = prop.GetValue(element);
                if (value == null)
                    continue;

                if (value == element)
                    continue;

                var UIC = (IUIComponent)value;
                if (!UIC.HasValue())
                    continue;
                list.Add(UIC);
                list.AddRange(UIC.GetAllChildren());
            }
            if (prop.PropertyType.IsAssignableTo(typeof(IEnumerable)) && prop.PropertyType != typeof(string))
            {
                var subList = (IEnumerable)prop.GetValue(element);
                if (subList == null)
                    continue;

                foreach (var value in subList)
                {
                    if (value == null)
                        continue;
                    if (value == element)
                        continue;
                    if (value is IUIComponent componentValue)
                    {
                        if (!componentValue.HasValue())
                            continue;

                        list.Add(componentValue);
                        list.AddRange(componentValue.GetAllChildren());
                    }
                }
            }

        }
        return list.Where(x => x != null).ToList();
    }

    public static void AssignParent(this IUIComponent element, IUIComponent parent)
    {
        if (element != null && element is IUICHasParent hasParent)
        {
            if (hasParent.Parent == null)
                hasParent.Parent = parent;
        }
    }

    /// <summary>
    /// This will check if the element is not null, and if the element has the Render property false, it will also return false
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static bool HasValue(this IUIComponent element)
    {
        if (element == null)
            return false;

        if (element.TryGetPropertyValue(nameof(UIComponent.Render), out bool render))
            return render;

        return true;
    }

    public static bool AnyHasValue(this IEnumerable<IUIComponent> elements)
    {
        foreach (var element in elements)
        {
            if(element.HasValue()) 
                return true;
        }
        return false;
    }

    /// <summary>
    /// Find the first element on its type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="element"></param>
    /// <param name="searchType"></param>
    /// <returns></returns>
    public static T FindFirstOnType<T>(this IUIComponent element) where T : IUIComponent
    {
        if (element is T onType)
            return onType;

        return element.FindFirstChildOnType<T>();
    }


    /// <summary>
    /// Find all types on this element
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="element"></param>
    /// <param name="searchType"></param>
    /// <returns></returns>
    public static List<T> FindAllOnType<T>(this IUIComponent element) where T : IUIComponent
    {
        var results = new List<T>();
        if (element is T ofType)
            results.Add(ofType);

        results.AddRange(element.FindAllChildrenOnType<T>());
        return results;
    }

    /// <summary>
    /// Find the first CHILD element on its type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="element"></param>
    /// <param name="searchType"></param>
    /// <returns></returns>
    public static T FindFirstChildOnType<T>(this IUIComponent element) where T : IUIComponent
    {
        return element.FindAllChildrenOnType<T>().FirstOrDefault();
    }

    /// <summary>
    /// Find all types on this element
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="element"></param>
    /// <param name="searchType"></param>
    /// <returns></returns>
    public static List<T> FindAllChildrenOnType<T>(this IUIComponent element) where T : IUIComponent
    {
        var typeResults = element.GetAllChildren().Where(x => x.GetType().IsAssignableTo(typeof(T))).OfType<T>().ToList();
        return typeResults;
    }

    public static T FindInputByPropertyName<T>(this IUIComponent element, string propertyName) where T : UICInput
    {
        var typeResults = element.GetAllChildren().Where(x => x.GetType().IsAssignableTo(typeof(T))).OfType<T>().ToList();
        return typeResults.Where(x => x.PropertyName.ToLower() == propertyName.ToLower()).FirstOrDefault();
    }


    public static string GetOrGenerateId(this IUIHasAttributes element)
    {
        string id = element.GetAttribute("id");
        if (string.IsNullOrWhiteSpace(id))
        {
            id = $"uic{Guid.NewGuid().ToString("N").Substring(0, 4)}";
            element.AddAttribute("id", id);
        }
        return id;
    }

    

    /// <summary>
    /// Get a value from the attributes
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetAttribute(this IUIHasAttributes element, string key)
    {
        key = key.ToLower();
        element.Attributes.TryGetValue(key, out string existing);
        return existing;
    }


    public static T AddAttribute<T>(this T element, string key, string value) where T : IUIHasAttributes
    {
        key = key.ToLower();

        if (!element.Attributes.TryGetValue(key, out string existing))
            existing = "";

        if (key.Contains("ident") && !string.IsNullOrEmpty(existing))
        {

        }

        if (!string.IsNullOrEmpty(existing) && (existing == value || existing.Contains($" {value} ") || existing.StartsWith($"{value} ") || existing.EndsWith($" {value}")))
            return element;

        if (string.IsNullOrEmpty(existing))
            existing = value;
        else
            existing = string.Join(" ", existing, value);

        element.Attributes[key] = existing;
        return element;
    }

    public static T AddClass<T>(this T element, string @class) where T : IUIHasAttributes
    {
        return element.AddAttribute("class", @class);
    }

    public static T AddStyle<T>(this T element, string style) where T : IUIHasAttributes
    {
        return element.AddAttribute("style", style);
    }

    /// <summary>
    /// This calls the <see cref="UIComponent.DefaultIdentifier(string, object)"/> where the <see cref="Type"/> of <paramref name="component"/> is used as first property
    /// </summary>
    public static string CreateDefaultIdentifier(this IUIComponent component, object renderer = null)
    {
        string name = component.GetType().Name;
        if (name.StartsWith("UIC"))
            name = name.Substring(3);

        return UIComponent.DefaultIdentifier(name, renderer);
    }

    public static bool TryGetPropertyValue<T>(this object obj, string propertyName, out T value)
    {
        value = default;
        try
        {
            var property = obj.GetType().GetProperty(propertyName);
            if (property == null)
                return false;

            if (!property.PropertyType.IsAssignableTo(typeof(T)))
                return false;

            value = (T)property.GetValue(obj, null);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
