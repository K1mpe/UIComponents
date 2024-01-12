using System.ComponentModel.DataAnnotations;

namespace UIComponents.ComponentModels.Models.Inputs;

/// <summary>
/// This is a input for a <see cref="string"/>
/// </summary>
public class UICInputText : UICInput<string>
{
    #region Fields
    public override bool HasClientSideValidation => ValidationRequired || ValidationMinLength.HasValue || ValidationMaxLength.HasValue;
    #endregion

    #region Ctor
    public UICInputText(string propertyName) : base(propertyName)
    {
    }

    public UICInputText() : this(null)
    {

    }



    #endregion

    #region Properties
    public DataType Type { get; set; } = DataType.Text;

    public bool ValidationRequired { get; set; }
    public int? ValidationMinLength { get; set; }
    public int? ValidationMaxLength { get; set; }

    #endregion


}
