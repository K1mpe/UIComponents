using UIComponents.Models.Interfaces;

namespace UIComponents.Models.Extensions;

public static class DropdownExtensions
{
    public static bool HasIcon(IHasIcon dropdownWithIcon)
    {
        var icon = dropdownWithIcon.Icon;
        if (icon == null)
            return false;
        if (icon.Render == false)
            return false;
        return true;
    }
}
