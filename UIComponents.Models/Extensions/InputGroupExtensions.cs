using UIComponents.Abstractions.Extensions;

namespace UIComponents.Models.Extensions;

public static class InputGroupExtensions
{

    public static UICInputGroup FindInputGroupByPropertyName(this IUIComponent element, string propertyName)
    {
        return element.FindInputGroupsByPropertyName(propertyName).FirstOrDefault();
    }
    public static List<UICInputGroup> FindInputGroupsByPropertyName(this IUIComponent element, string propertyName)
    {
        if (element == null)
            return null;
        var typeResults = element.GetAllChildren().Where(x => x.GetType().IsAssignableTo(typeof(UICInputGroup))).OfType<UICInputGroup>().ToList();
        return typeResults.Where(x => x.Input != null && x.Input.PropertyName != null && x.Input.PropertyName.ToLower() == propertyName.ToLower()).ToList();
    }
}
