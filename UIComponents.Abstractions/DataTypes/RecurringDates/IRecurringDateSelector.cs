namespace UIComponents.Abstractions.DataTypes.RecurringDates;

public interface IRecurringDateSelector : IUIComponent
{
    #region Properties
    /// <summary>
    /// This is to check if this dateselector is incomplete, Invalid selectors are not saved.
    /// </summary>
    [UICIgnore]
    public bool IsInvalid { get; }
    #endregion


    #region Methods


    public DateOnly? GetNextDate(RecurringDateItem dateItem, DateOnly start);
    public bool IsValidDate(RecurringDateItem dateItem, DateOnly date);

    public string Serialize();
    public IRecurringDateSelector Deserialize(string serialised);

    #endregion
}
