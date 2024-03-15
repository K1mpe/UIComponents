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
    public static List<UICInputGroup> FindInputGroupsByPropertyName(this IUIComponent element, string propertyName)
    {
        if (element == null)
            return null;
        var typeResults = element.GetAllChildren().Select(x=>x.Component).Where(x => x.GetType().IsAssignableTo(typeof(UICInputGroup))).OfType<UICInputGroup>().ToList();
        return typeResults.Where(x => x.Input != null && x.Input.PropertyName != null && x.Input.PropertyName.ToLower() == propertyName.ToLower()).ToList();
    }
}
