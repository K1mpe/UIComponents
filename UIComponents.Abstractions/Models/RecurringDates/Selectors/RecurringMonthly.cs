using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
namespace UIComponents.Abstractions.Models.RecurringDates.Selectors;

public class RecurringMonthly : IRecurringDateSelector
{
    #region Fields
    [UICIgnore]
    public string RenderLocation => "/UIComponents/RecurringDateTypes/RecurringMonthly";

    #endregion

    #region Properties
    /// <summary>
    /// A int indicating what style of recurring is used.
    /// </summary>
    /// <remarks>
    /// <br>0: Sunday</br>
    /// <br>1: Monday</br>
    /// <br>2: Tuesday</br>
    /// <br>3: Wednesday</br>
    /// <br>4: Thrusday</br>
    /// <br>5: Friday</br>
    /// <br>6: Saturday</br>
    /// <br>10: Day</br>
    /// <br>11: WeekDay</br>
    /// <br>12: Weekend</br>
    /// </remarks>
    public int RecurringStyle { get; set; }

    [Range(1, int.MaxValue)]
    public int EveryXMonths { get; set; } = 1;
    public MonthlyInstance Instance { get; set; }

    public bool IsInvalid => RecurringStyle < 1 || Instance == null;
    #endregion

    #region Methods


    public DateOnly? GetNextDate(RecurringDateItem dateItem, DateOnly start)
    {
        throw new NotImplementedException();
    }

    public bool IsValidDate(RecurringDateItem dateItem, DateOnly date)
    {
        if(RecurringStyle < 7)
        {
            if (RecurringStyle != (int)date.DayOfWeek)
                return false;
        }
        else
        {
            if (RecurringStyle == 11 && (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday))
                return false;

            if (RecurringStyle == 12 && (date.DayOfWeek != DayOfWeek.Sunday && date.DayOfWeek != DayOfWeek.Saturday))
                return false;
        }

        switch (Instance)
        {
            case MonthlyInstance.First:
                if (date.Day > 7)
                    return false;
                break;
            case MonthlyInstance.Second:
                if (date.Day <= 7 && date.Day > 14)
                    return false;
                break;
            case MonthlyInstance.Third:
                if (date.Day <= 14 && date.Day > 21)
                    return false;
                break;
            case MonthlyInstance.Forth:
                if (date.Day <= 21 && date.Day > 28)
                    return false;
                break;
            case MonthlyInstance.Last:
                var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
                if (date.Day <= daysInMonth - 7)
                    return false;
                break;
        }

        if (EveryXMonths == 1)
            return true;


        int months = (date.Year - dateItem.StartDate.Year) * 12;
        months += (date.Month - dateItem.StartDate.Month);
        return months % EveryXMonths == 0;
    }

    public string Serialize()
    {
        string serialised= JsonSerializer.Serialize(this);
        Console.WriteLine(serialised);
        return serialised;
    }

    public IRecurringDateSelector Deserialize(string serialized)
    {
        var dictionary = RecurringDateItem.DeserializeDict(serialized);
        RecurringStyle = int.Parse(dictionary[nameof(RecurringStyle)]);
        Instance = Enum.Parse<MonthlyInstance>(dictionary[nameof(Instance)]);
        EveryXMonths = int.Parse(dictionary[nameof(EveryXMonths)]);
        return this;
    }
    #endregion


    public enum MonthlyInstance
    {
        First = 1,
        Second = 2,
        Third = 3,
        Forth = 4,
        Last = 5,
    }
}
