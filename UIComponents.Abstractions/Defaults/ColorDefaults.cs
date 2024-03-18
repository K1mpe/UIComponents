namespace UIComponents.Defaults;

public static class ColorDefaults
{
    public static IColor DefaultColor { get; set; } = new UICColor("default");
    public static IColor PrimaryColor { get; set; } = new UICColor("primary");

    public static IColor ButtonDefault { get; set; } = DefaultColor;
    public static IColor ButtonSubmit { get; set; } = PrimaryColor;
    public static IColor ButtonDelete { get; set; } = ButtonDefault;

    public static IColor InputCheckbox { get; set; }

    public static IColor CardHeaderDefault { get; set; }
}