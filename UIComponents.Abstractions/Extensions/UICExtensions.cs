using System.Collections;
using System.Collections.Generic;
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
    public static List<(IUIComponent Component, IUIComponent Parent)> GetAllChildren(this IUIComponent element)
    {
        var list = new List<(IUIComponent, IUIComponent)>();
        if (element == null)
            return list;
        foreach (var prop in element.GetType().GetProperties())
        {
            if (prop.GetCustomAttribute<UICIgnoreGetChildrenFunctionAttribute>() != null)
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
                list.Add(new(UIC, element));
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

                        list.Add(new(componentValue, element));
                        list.AddRange(componentValue.GetAllChildren());
                    }
                }
            }

        }
        return list.Where(x => x.Item1 != null).ToList();
    }

    /// <summary>
    /// Remove a direct child of this component, if this component contains this child.
    /// </summary>
    public static void RemoveComponent(this IUIComponent parent, IUIComponent child) 
    {
        foreach (var prop in parent.GetType().GetProperties())
        {
            if (prop.GetCustomAttribute<UICIgnoreGetChildrenFunctionAttribute>() != null)
                continue;

            if (prop.PropertyType.IsAssignableTo(typeof(IUIComponent)))
            {
                var value = prop.GetValue(parent);
                if (value == child)
                    value = null;
                    continue;
            }
            if (prop.PropertyType.IsAssignableTo(typeof(IList)) && prop.PropertyType != typeof(string))
            {
                var subList = (IList)prop.GetValue(parent);
                if (subList == null)
                    continue;

                for(int i = 0; i < subList.Count; i++)
                {
                    var value = subList[i];
                    if (value == child)
                    {
                        subList[i] = null;
                        break;
                    }
                        
                }
            }

        }
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
    public static T FindFirstOfType<T>(this IUIComponent element, Action<T> action = null) where T : IUIComponent
    {
        if (element is T onType)
        {
            if (action != null)
                action(onType);
            return onType;
        }
        
        return element.FindFirstChildOfType<T>(action);
    }

    public static bool TryFindFirstOfType<T>(this IUIComponent element, Action<T> action) where T : IUIComponent
    {
        try
        {
            var result = FindFirstOfType<T>(element);
            if(result == null)
                return false;

            action(result);
            return true;
        }
        catch
        {
            return false;
        }
    }
    public static bool TryFindFirstOfType<T>(this IUIComponent element, out T result) where T : IUIComponent
    {
        try
        {
            result = FindFirstOfType<T>(element);
            return result != null;
        }
        catch
        {
            result = default(T);
            return false;
        }
    }

    /// <summary>
    /// Find all types on this element
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="element"></param>
    /// <param name="searchType"></param>
    /// <returns></returns>
    public static List<T> FindAllOfType<T>(this IUIComponent element) where T : IUIComponent
    {
        var results = new List<T>();
        if (element is T ofType)
            results.Add(ofType);

        results.AddRange(element.FindAllChildrenOfType<T>());
        return results;
    }

    /// <summary>
    /// Find the first CHILD element on its type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="element"></param>
    /// <param name="searchType"></param>
    /// <returns></returns>
    public static T FindFirstChildOfType<T>(this IUIComponent element, Action<T> action = null) where T : IUIComponent
    {
        var first = element.FindAllChildrenOfType<T>().FirstOrDefault();
        if (action != null)
            action(first);
        return first;
    }
    public static bool TryFindChildFirstOfType<T>(this IUIComponent element, Action<T> action) where T : IUIComponent
    {
        try
        {
            var result = FindFirstChildOfType<T>(element);
            if (result == null)
                return false;

            action(result);
            return true;
        }
        catch
        {
            return false;
        }
    }
    public static bool TryFindChildFirstOfType<T>(this IUIComponent element, out T result) where T : IUIComponent
    {
        try
        {
            result = FindFirstChildOfType<T>(element);
            return result != null;
        }
        catch
        {
            result = default(T);
            return false;
        }
    }
    /// <summary>
    /// Find all types on this element
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="element"></param>
    /// <param name="searchType"></param>
    /// <returns></returns>
    public static List<T> FindAllChildrenOfType<T>(this IUIComponent element) where T : IUIComponent
    {
        var typeResults = element.GetAllChildren().Select(x=>x.Component).Where(x => x.GetType().IsAssignableTo(typeof(T))).OfType<T>().ToList();
        return typeResults;
    }

    public static T FindInputByPropertyName<T>(this IUIComponent element, string propertyName, Action<T> action= null) where T : UICInput
    {
        var typeResults = element.GetAllChildren().Select(x=>x.Component).Where(x => x.GetType().IsAssignableTo(typeof(T))).OfType<T>().ToList();
        var firstInput =  typeResults.Where(x => x.PropertyName.ToLower() == propertyName.ToLower()).FirstOrDefault();

        if(action != null && firstInput != null)
            action(firstInput);
        return firstInput;
    }
    public static bool TryFindInputByPropertyName<T>(this IUIComponent element, string propertyName, Action<T> action) where T : UICInput
    {
        try
        {
            var result = FindInputByPropertyName<T>(element, propertyName);
            if (result == null)
                return false;

            action(result);
            return true;
        }
        catch
        {
            return false;
        }
    }
    public static bool TryFindInputByPropertyName<T>(this IUIComponent element, string propertyName, out T result) where T : UICInput
    {
        try
        {
            result = FindInputByPropertyName<T>(element, propertyName);
            return result != null;
        }
        catch
        {
            result = default(T);
            return false;
        }
    }
    /// <summary>
    /// Gets the Id from a element. Create a random id if none exists yet.
    /// </summary>
    /// <param name="element"></param>
    /// <returns></returns>
    public static string GetId(this IUICHasAttributes element)
    {
        string id = element.GetAttribute("id");
        if (string.IsNullOrWhiteSpace(id))
        {
            id = $"uic{Guid.NewGuid().ToString("N")}";
            element.AddAttribute("id", id);
        }
        return id;
    }
    /// <summary>
    /// Set the Id for this element. Will throw <see cref="Exception"></see> if the item already has a id.
    /// </summary>
    /// <exception cref="Exception"></exception>
    public static T SetId<T>(this T element, string id) where T : IUICHasAttributes
    {
        if(id.StartsWith("#"))
            id = id.Substring(1);

        string existingId = element.GetAttribute("id");
        if (!string.IsNullOrEmpty(existingId))
            throw new Exception($"Element already contains a Id. {element}, {id}, {existingId}");

        element.AddAttribute("id", id);
        return element;
    }

    /// <summary>
    /// Get a value from the attributes
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string GetAttribute(this IUICHasAttributes element, string key)
    {
        key = key.ToLower();
        element.Attributes.TryGetValue(key, out string existing);
        return existing;
    }


    public static T AddAttribute<T>(this T element, string key, string value) where T : IUICHasAttributes
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

    public static T AddClass<T>(this T element, string @class) where T : IUICHasAttributes
    {
        return element.AddAttribute("class", @class);
    }

    public static T AddCss<T>(this T element, string name, string value) where T : IUICHasAttributes
    {
        return element.AddAttribute("style", $"{name}: {value};");
    }

    public static T AddStyle<T>(this T element, string style) where T : IUICHasAttributes
    {
        return element.AddAttribute("style", style);
    }

    /// <summary>
    /// This calls the <see cref="UIComponent.DefaultIdentifier(string, object)"/> where the <see cref="Type"/> of <paramref name="component"/> is used as first property
    /// </summary>
    public static string CreateDefaultIdentifier(this IUIComponent component, object renderer = null)
    {
        string name = component.GetType().Name;

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
