using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Inputs;

public class UICInputDateRange : UICInput<IDateRangeInput>
{
    #region Fields
    public override bool HasClientSideValidation => ValidationRequired || ValidationMinimumDate.HasValue || ValidationMaximumDate.HasValue || ValidationMaxLength.HasValue;

    #endregion

    #region Ctor
    public UICInputDateRange() : this(null)
    {

    }
    public UICInputDateRange(string propertyName) : base(propertyName)
    {

    }
    #endregion

    #region Properties

    /// <summary>
    /// Choose to show date only, minutes or seconds
    /// </summary>
    public UICDatetimeStep Precision { get; set; } = UICDatetimeStep.Second;

    /// <summary>
    /// If true, this inputfield will force the min-Width to the minimum required width.
    /// </summary>
    /// <remarks>
    /// This does not work when the input already has a "min-width" attribute.
    /// </remarks>
    public bool ForceFitInput { get; set; } = false;

    /// <summary>
    /// Show the weeknumbers on the calendar
    /// </summary>
    public bool ShowWeeknumbers { get; set; }

    public bool AutoApply { get; set; }

    /// <summary>
    /// These are predefined ranges that can be selected. F.e. Last 7 Days, Last 30 Days, etc
    /// </summary>
    public List<DateRangeSelector> RangeSelectors { get; set; } = new();

    /// <summary>
    /// Hide the calendar when there are RangeSelectors available.
    /// </summary>
    /// <remarks>
    /// There is always a option to manualy select dates, this will open the calendar
    /// </remarks>
    public bool AlwaysShowCalendar { get; set; }

    /// <summary>
    /// Overwrite the default displayformat
    /// </summary>
    public string DisplayFormat { get; set; }

    /// <summary>
    /// If there are 2 calendars shown to select the date, disconnect you can navigate them indevidualy
    /// </summary>
    public bool DisconnectCalendars { get; set; }


    /// <summary>
    /// Additional options supported by <see href="https://www.daterangepicker.com/"/>
    /// </summary>
    public Dictionary<string, object> Options { get; set; } = new();

    public bool ValidationRequired { get; set; }

    public DateTime? ValidationMinimumDate { get; set; }
    public DateTime? ValidationMaximumDate { get; set; }
    public TimeSpan? ValidationMaxLength { get; set; }
    #endregion



}

public class DateRangeSelector
{
    public Translatable Title { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public static DateRangeSelector Today()
    {
        return new DateRangeSelector()
        {
            Title = TranslatableSaver.Save("DateRangeSelector.Today"),
            Start = DateTime.Today,
            End = DateTime.Today.AddDays(1).AddMilliseconds(-1),
        };
    }

    public static DateRangeSelector Last7Days()
    {
        return new DateRangeSelector()
        {
            Title = TranslatableSaver.Save("DateRangeSelector.Last7Days"),
            Start = DateTime.Today.AddDays(-7),
            End = DateTime.Now,
        };
    }

    public static DateRangeSelector ThisMonth()
    {
        var now = DateTime.Now;
        return new DateRangeSelector()
        {
            Title = TranslatableSaver.Save("DateRangeSelector.Last7Days"),
            Start = new DateTime(now.Year, now.Month, 1),
            End = now,
        };
    }
}

public class DateRangeInput : IDateRangeInput
{
    public DateRangeInput(DateTime start, DateTime end )
    {
        From = start;
        To = end;
    }

    public DateTime? From { get; set; }

    public DateTime? To { get; set; }
}
