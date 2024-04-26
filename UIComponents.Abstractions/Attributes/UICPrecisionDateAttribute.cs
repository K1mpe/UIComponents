using UIComponents.Abstractions.Enums;

namespace UIComponents.Abstractions.Attributes;

public class UICPrecisionDateAttribute : Attribute
{
    /// <summary>
    /// Attribute that is used to define the precision in visualisation for <see cref="DateTime"/> or <see cref="DateOnly"/> properties
    /// </summary>
    /// <param name="precision"></param>
    public UICPrecisionDateAttribute(UICDatetimeStep precision)
    {
        Precision = precision;
    }

    public UICDatetimeStep Precision { get; set; }

    
}
