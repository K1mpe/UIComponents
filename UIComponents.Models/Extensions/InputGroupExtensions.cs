using UIComponents.Abstractions.Extensions;

namespace UIComponents.Models.Extensions;

public static class InputGroupExtensions
{

    public static UICInputGroup FindInputGroupByPropertyName(this IUIComponent element, string propertyName, Action<UICInputGroup> action = null)
    {
        var first = element.FindInputGroupsByPropertyName(propertyName).FirstOrDefault();
        if (action != null)
            action(first);
        return first;
    }
    public static bool TryFindInputGroupByPropertyName(this IUIComponent element, string propertyName, Action<UICInputGroup> action)
    {
        try
        {
            var result = FindInputGroupByPropertyName(element, propertyName);
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
    public static bool TryFindInputGroupByPropertyName(this IUIComponent element, string propertyName, out UICInputGroup result)
    {
        try
        {
            result = FindInputGroupByPropertyName(element, propertyName);
            return result != null;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    public static List<UICInputGroup> FindInputGroupsByPropertyName(this IUIComponent element, string propertyName)
    {
        if (element == null)
            return null;
        var typeResults = element.GetAllChildren().Select(x=>x.Component).Where(x => x.GetType().IsAssignableTo(typeof(UICInputGroup))).OfType<UICInputGroup>().ToList();
        return typeResults.Where(x => x.Input != null && x.Input.PropertyName != null && x.Input.PropertyName.ToLower() == propertyName.ToLower()).ToList();
    }
}
