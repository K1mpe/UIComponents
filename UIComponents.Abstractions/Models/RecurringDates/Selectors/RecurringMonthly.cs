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
        if (IsValidDate(dateItem, start))
            return start;

        var date = start;
        int i = 0; // i is failsafe in case of infinite loop, should always end loop before this
        while (i <10000 && (dateItem.EndDate == null || date <= dateItem.EndDate))
        {
            int months = (date.Year - dateItem.StartDate.Year) * 12;
            months += (date.Month - dateItem.StartDate.Month);

            var modelo = months % EveryXMonths;
            if(modelo > 0)
            {
                //Start op day 1 of the month
                int revertDays = -(date.Day-1);
                switch (Instance)
                {
                    //Depending on the instance, move the date a certain amount of days forward
                    case MonthlyInstance.First:
                        break;
                    case MonthlyInstance.Second:
                        revertDays += 7;
                        break;
                    case MonthlyInstance.Third:
                        revertDays += 14;
                        break;
                    case MonthlyInstance.Forth:
                    case MonthlyInstance.Last:
                        revertDays += 21;
                        break;
                }
                date = date.AddMonths(EveryXMonths - modelo).AddDays(revertDays);
                continue;
            }

            var day = date.Day;
            switch (Instance)
            {
                case MonthlyInstance.First:
                    if(day > 7)
                    {
                        var setDay = -(day - 1);
                        date = date.AddMonths(1).AddDays(setDay);
                        continue;
                    }
                    break;
                case MonthlyInstance.Second:
                    if (day <=7 || day > 14)
                    {
                        var setDay = -(day - 1);
                        setDay += 7;
                        date = date.AddDays(setDay);
                        if (day > 14)
                            date = date.AddMonths(1);
                        continue;
                    }
                    break;
                case MonthlyInstance.Third:
                    if (day <= 14 || day > 21)
                    {
                        var setDay = -(day - 1);
                        setDay += 14;
                        date = date.AddDays(setDay);
                        if (day > 21)
                            date = date.AddMonths(1);
                        continue;
                    }
                    break;
                case MonthlyInstance.Forth:
                    if (day <= 21 || day > 28)
                    {
                        var setDay = -(day - 1);
                        setDay += 21;
                        date = date.AddDays(setDay);
                        if (day > 28)
                            date = date.AddMonths(1);
                        continue;
                    }
                    break;
                case MonthlyInstance.Last:
                    var daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);
                    if (date.Day <= daysInMonth - 7)
                    {
                        var setDay = -(day - 1);
                        setDay += (daysInMonth-7);
                        date = date.AddDays(setDay);
                        continue;
                    }
                    break;
            }

            if(RecurringStyle < 7)
            {
                if (RecurringStyle == (int)date.DayOfWeek)
                    return date;
                else
                {
                    date = date.AddDays(1);
                    continue;
                }
            }
            if (RecurringStyle == 10)
                return date;
            if(RecurringStyle == 11)
            {
                if (date.DayOfWeek != DayOfWeek.Sunday && date.DayOfWeek != DayOfWeek.Saturday)
                    return date;
                else
                {
                    date = date.AddDays(1);
                    continue;
                }
            }
            if(RecurringStyle == 12)
            {
                if (date.DayOfWeek == DayOfWeek.Sunday || date.DayOfWeek == DayOfWeek.Saturday)
                    return date;
                else
                {
                    date = date.AddDays(1);
                    continue;
                }
            }
        }
        return null;
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
