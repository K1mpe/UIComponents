namespace UIComponents.ComponentModels.Models.Inputs;

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
    public bool ValidationRequired { get; set; }
    public int? ValidationMinLength { get; set; }
    public int? ValidationMaxLength { get; set; }

    #endregion

}
