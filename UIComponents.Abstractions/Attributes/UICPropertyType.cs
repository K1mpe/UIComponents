namespace UIComponents.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class UICPropertyTypeAttribute : Attribute
{
    public UICPropertyTypeAttribute(UICPropertyType type)
    {
        Type = type;
    }

    public UICPropertyType Type { get; }
}
public enum UICPropertyType
{
    /// <summary>
    /// string input field
    /// </summary>
    /// <remarks>
    /// Default for: strings
    /// </remarks>
    String,

    /// <summary>
    /// Textbox on multiple lines (CrudForm only)
    /// </summary>
    MultilineText,

    /// <summary>
    /// Selectlist for foreign key or enums
    /// </summary>
    /// <remarks>
    /// Default for: int or long that ends with Id, ForeignKeyAttribute or FakeForeignkeyAttribute
    /// </remarks>
    SelectList,

    /// <summary>
    /// A whole number
    /// </summary>
    /// <remarks>
    /// Default for: byte, int, long
    /// </remarks>
    Number,

    /// <summary>
    /// A number with decimal values
    /// </summary>
    /// <remarks>
    /// Default for: single, double, decimal
    /// </remarks>
    Decimal,

    /// <summary>
    /// A date-only field
    /// </summary>
    DateOnly,

    /// <summary>
    /// A Date-time field
    /// </summary>
    /// <remarks>
    /// Default for: DateTime
    /// </remarks>
    DateTime,

    /// <summary>
    /// A field to display time without date
    /// </summary>
    TimeOnly,

    /// <summary>
    /// A time duration
    /// </summary>
    TimeSpan,

    /// <summary>
    /// A boolean field
    /// </summary>
    /// <remarks>
    /// Default for: bool
    /// </remarks>
    Boolean,

    /// <summary>
    /// A boolean with 3 states, On, Off and Null
    /// </summary>
    /// <remarks>
    /// Default for: bool?
    /// </remarks>
    ThreeStateBoolean,

    /// <summary>
    /// A hexColor with colorpicker
    /// </summary>
    /// <remarks>
    /// Default for: strings containing "color" in the propertyname
    /// </remarks>
    HexColor,

}
