using UIComponents.Abstractions.Helpers;

namespace UIComponents.Models.Models.Inputs;

/// <summary>
/// This is a input for a <see cref="string"/>
/// <br>This has support for multiple lines of text</br>
/// </summary>
public class UICInputMultiline : UICInput<string>
{
    #region Fields
    public override bool HasClientSideValidation => ValidationRequired || ValidationMinLength.HasValue || ValidationMaxLength.HasValue;
    #endregion

    #region Ctor
    public UICInputMultiline() : this(null)
    {

    }
    public UICInputMultiline(string propertyName) : base(propertyName)
    {
    }


    #endregion

    #region Properties
    /// <summary>
    /// If not null, set the minimum amount of visible lines in the text area
    /// </summary>
    public int? MinRows { get; set; }


    /// <summary>
    /// If not null, set the maximum amount of visible lines in the text area.
    /// <br>If the text requires more lines, this will display a scrollbar</br>
    /// </summary>
    public int? MaxRows { get; set; }


    public bool ValidationRequired { get; set; }
    public int? ValidationMinLength { get; set; }
    public int? ValidationMaxLength { get; set; }

    #endregion

    #region Converters
    public UICInputText ConvertToSingleLine() => InternalHelper.ConvertObject<UICInputText>(this);
    #endregion
}
