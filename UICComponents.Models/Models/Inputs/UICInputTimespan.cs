namespace UIComponents.ComponentModels.Models.Inputs;

public class UICInputTimespan : UICInput<TimeSpan?>
{

    #region Fields

    public override bool HasClientSideValidation => ValidationRequired || ValidationMinValue != null || ValidationMaxValue != null;


    #endregion

    #region Ctor
    public UICInputTimespan() : base(null)
    {

    }

    public UICInputTimespan(string propertyName) : base(propertyName)
    {
    }

    #endregion

    #region Properties


    public bool ShowDays { get; set; } = true;
    public bool ShowHours { get; set; } = true;
    public bool ShowMinutes { get; set; } = true;
    public bool ShowSeconds { get; set; } = true;
    public bool ShowMilliseconds { get; set; }


    public bool ValidationRequired { get; set; }

    public TimeSpan? ValidationMinValue { get; set; }
    public TimeSpan? ValidationMaxValue { get; set; }
    #endregion
}
