using UIComponents.ComponentModels.Interfaces;

namespace UIComponents.ComponentModels.Extentions;

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
