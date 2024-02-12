using Microsoft.AspNetCore.Mvc.Rendering;

namespace UIComponents.Models.Extensions;

public static class SelectListExtensions
{
    public static List<UICSelectListItem> ToUIC(this IEnumerable<SelectListItem> source)
    {
        return source.Select(x => (UICSelectListItem)x).ToList();
    }
}
