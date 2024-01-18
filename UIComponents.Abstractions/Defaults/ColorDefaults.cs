using UIComponents.Abstractions.Interfaces;

namespace UIComponents.Defaults;

public static class ColorDefaults
{
    public static IColor DefaultColor { get; set; }

    public static IColor ButtonDefault { get; set; } = DefaultColor;
    public static IColor ButtonSave { get; set; } = ButtonDefault;
    public static IColor ButtonDelete { get; set; } = ButtonDefault;

    public static IColor InputCheckbox { get; set; }

    public static IColor CardHeaderDefault { get; set; } = DefaultColor;
}