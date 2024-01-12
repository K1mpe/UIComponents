using System.Drawing;

namespace UIComponents.Abstractions;

public interface IColor
{
    public string Name { get; set; }
    public string Hex { get; set; }
    public string ToLower()
    {
        return Name.ToLower();
    }
}

public static class Colors
{
    public static IColor DefaultColor { get; set; }

    public static IColor ButtonDefault { get; set; }
    public static IColor ButtonSave { get; set; }
    public static IColor ButtonDelete { get; set; }

    public static IColor InputCheckbox { get; set; }

    public static IColor CardHeaderDefault { get; set; }
}