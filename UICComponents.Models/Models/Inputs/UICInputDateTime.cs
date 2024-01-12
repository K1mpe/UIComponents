namespace UIComponents.ComponentModels.Models.Inputs;

/// <summary>
/// This is a input for a <see cref="DateTime"/>
/// </summary>
public class UICInputDatetime : UICInput<DateTime?>
{
    #region Fields
    public override bool HasClientSideValidation => ValidationRequired || ValidationMinimumDate.HasValue || ValidationMaximumDate.HasValue;
    #endregion

    #region ctor
    public UICInputDatetime() : this(null)
    {

    }
    public UICInputDatetime(string propertyName) : base(propertyName)
    {
    }
    #endregion

    #region Properties

    /// <summary>
    /// Choose to show date only, minutes or seconds
    /// </summary>
    public UICDatetimeStep Precision { get; set; }

    public bool ValidationRequired { get; set; }

    public DateTime? ValidationMinimumDate { get; set; }
    public DateTime? ValidationMaximumDate { get; set; }

    #endregion
}

