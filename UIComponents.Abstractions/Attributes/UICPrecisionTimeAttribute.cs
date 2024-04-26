using UIComponents.Abstractions.Enums;

namespace UIComponents.Abstractions.Attributes;

public class UICPrecisionTimeAttribute : Attribute
{
    /// <summary>
    /// Attribute that is used to define the precision in visualisation for <see cref="DateTime"/> or <see cref="TimeOnly"/> properties
    /// </summary>
    /// <param name="precision"></param>
    public UICPrecisionTimeAttribute(UICTimeonlyEnum precision)
    {
        Precision = precision;
    }

    public UICTimeonlyEnum Precision { get; set; }
}
