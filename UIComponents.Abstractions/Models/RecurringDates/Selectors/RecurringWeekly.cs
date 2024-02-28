
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace UIComponents.Abstractions.Models.RecurringDates.Selectors;

public class RecurringWeekly : IRecurringDateSelector
{
    #region Fields
    [UICIgnore]
    public string RenderLocation => "/UIComponents/RecurringDateTypes/RecurringWeekly";
    #endregion


    #region Properties
    [Range(1, int.MaxValue)]
    public int EveryXWeeks { get; set; } = 1;

    public bool Monday { get; set; }
    public bool Tuesday { get; set; }
    public bool Wednesday { get; set; }
    public bool Thursday { get; set; }
    public bool Friday { get; set; }
    public bool Saturday { get; set; }
    public bool Sunday { get; set; }


    [UICIgnore]
    public List<DayOfWeek> SelectedDays
    {
        get
        {
            List<DayOfWeek> setDays = new List<DayOfWeek>();
            if (Monday)
                setDays.Add(DayOfWeek.Monday);
            if (Tuesday)
                setDays.Add(DayOfWeek.Tuesday);
            if (Wednesday)
                setDays.Add(DayOfWeek.Wednesday);
            if (Thursday)
                setDays.Add(DayOfWeek.Thursday);
            if (Friday)
                setDays.Add(DayOfWeek.Friday);
            if (Saturday)
                setDays.Add(DayOfWeek.Saturday);
            if (Sunday)
                setDays.Add(DayOfWeek.Sunday);

            return setDays;
        }
    }

    [UICIgnore]
    public bool IsInvalid => !Monday && !Tuesday && !Wednesday && !Thursday && !Friday && !Saturday && !Sunday;




    #endregion

    #region Methods

    public DateOnly? GetNextDate(RecurringDateItem dateItem, DateOnly start)
    {
        var startDayNr = dateItem.StartDate.DayNumber;
        var date = start;
        while (true)
        {
            var dayNr = date.DayNumber;
            var daysPast = dayNr - startDayNr;
            var remainingWeeks = Math.Floor(daysPast/7f) % EveryXWeeks;
            if (remainingWeeks >1)
            {
                date = date.AddDays(7 * ((int)remainingWeeks - 1));
                continue;
            }
            if(remainingWeeks == 0)
            {
                var dayOfWeek = date.DayOfWeek;
                if (Monday && dayOfWeek == DayOfWeek.Monday)
                    return date;
                if (Tuesday && dayOfWeek == DayOfWeek.Tuesday)
                    return date;
                if (Wednesday && dayOfWeek == DayOfWeek.Wednesday)
                    return date;
                if (Thursday && dayOfWeek == DayOfWeek.Thursday)
                    return date;
                if (Friday && dayOfWeek == DayOfWeek.Friday)
                    return date;
                if (Saturday && dayOfWeek == DayOfWeek.Saturday)
                    return date;
                if (Sunday && dayOfWeek == DayOfWeek.Sunday)
                    return date;
            }
            
            date = date.AddDays(1);
        }

    }

    public bool IsValidDate(RecurringDateItem dateItem, DateOnly date)
    {
        var startDayNr = dateItem.StartDate.DayNumber;
        var dayNr = date.DayNumber;
        var daysPast = dayNr - startDayNr;
        var remainingWeeks = Math.Floor(daysPast / 7f) % EveryXWeeks;

        if (remainingWeeks != 0)
            return false;

        var dayOfWeek = date.DayOfWeek;
        if (Monday && dayOfWeek == DayOfWeek.Monday)
            return true;
        if (Tuesday && dayOfWeek == DayOfWeek.Tuesday)
            return true;
        if (Wednesday && dayOfWeek == DayOfWeek.Wednesday)
            return true;
        if (Thursday && dayOfWeek == DayOfWeek.Thursday)
            return true;
        if (Friday && dayOfWeek == DayOfWeek.Friday)
            return true;
        if (Saturday && dayOfWeek == DayOfWeek.Saturday)
            return true;
        if (Sunday && dayOfWeek == DayOfWeek.Sunday)
            return true;

        return false;
        
    }

    public string Serialize()
    {
        var serialized = JsonSerializer.Serialize(this);
        return serialized;
    }

    public IRecurringDateSelector Deserialize(string serialized)
    {
        var dict = RecurringDateItem.DeserializeDict(serialized);
        EveryXWeeks = int.Parse(dict[nameof(EveryXWeeks)]);
        Monday = bool.Parse(dict[nameof(Monday)]);
        Tuesday = bool.Parse(dict[nameof(Tuesday)]);
        Wednesday = bool.Parse(dict[nameof(Wednesday)]);
        Thursday = bool.Parse(dict[nameof(Thursday)]);
        Friday = bool.Parse(dict[nameof(Friday)]);
        Saturday = bool.Parse(dict[nameof(Saturday)]);
        Sunday = bool.Parse(dict[nameof(Sunday)]);
        return this;
    }
    #endregion
}
