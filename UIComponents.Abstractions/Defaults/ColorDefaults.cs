namespace UIComponents.Defaults;

public static class ColorDefaults
{
    public static Func<IColor> DefaultColor { get; set; } = () => new UICColor("default");
    public static Func<IColor> PrimaryColor { get; set; } = ()=> new UICColor("primary");

    public static Func<IColor> ButtonDefault { get; set; } = DefaultColor;
    public static Func<IColor> ButtonSubmit { get; set; } = PrimaryColor;
    public static Func<IColor> ButtonDelete { get; set; } = ButtonDefault;

    public static Func<IColor> InputCheckbox { get; set; }

    public static Func<IColor> CardHeaderDefault { get; set; }
}