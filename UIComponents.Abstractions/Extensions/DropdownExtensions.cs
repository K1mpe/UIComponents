namespace UIComponents.Abstractions.Extensions;

public static class IconExtensions
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
