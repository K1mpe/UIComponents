using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static UIComponents.Abstractions.Models.RecurringDates.Selectors.RecurringMonthly;

namespace UIComponents.Abstractions.Models.RecurringDates.Selectors
{
    public class RecurringCustomDate : IRecurringDateSelector
    {
        #region Fields
        public bool IsInvalid => !Days.Any() && !Months.Any()&& !Years.Any();
        public string RenderLocation => "/UIComponents/ComponentViews/RecurringDateTypes/RecurringCustom";
        #endregion

        #region Properties
        public List<int> Days { get; set; } = new();
        public List<int> Months { get; set; } = new();
        public List<int> Years { get; set; } = new();

        #endregion


        #region Methods


        public DateOnly? GetNextDate(RecurringDateItem dateItem, DateOnly start)
        {
            if (IsValidDate(dateItem, start))
                return start;
            Years = Years.OrderBy(x => x).ToList();
            Months = Months.OrderBy(x => x).Where(x=>x >0 && x <=12).ToList();
            Days = Days.OrderBy(x => x).Where(x=> x >0 && x <= 31).ToList();


            int? year = start.Year;
            int month = start.Month;
            int day = start.Day;
            year = GetYear(year);
            month = GetMonth(month);
            day = GetDay(day);
            if (year == null)
                return null;
            int counter = 0;
            while(DateTime.DaysInMonth(year.Value, month) < day && counter <1000)
            {
                day = GetDay(day+1);
                counter++;
            }
            if (counter == 1000)
                return null;

            return new DateOnly(year.Value, month, day);

            int GetDay(int day)
            {
                if(day > 31)
                {
                    month = GetMonth(month+1);
                    return Days.Any() ? Days.First() : 1;
                }
                if (!Days.Any())
                    return day;
                if (Days.Contains(day))
                    return day;

                if(Days.Where(x=> x > day).Any())
                    return Days.Where(x=> x >day).First();
                else
                {
                    month = GetMonth(month + 1);
                    return Days.First();
                }
            }
            int GetMonth(int month)
            {
                if (!Months.Any())
                    return month;

                
                if (Months.Any(x => x >= start.Month))
                    return Months.Where(x => x >= start.Month).First();
                else
                {
                    year = GetYear(year + 1);
                    day = GetDay(1);
                    return Months.First();
                }
            }
            int? GetYear(int? year)
            {
                if (year == null)
                    return null;

                if (Years.Any())
                {
                    if (Years.Contains(year.Value))
                        return year;

                    if (!Years.Any(x => x >= year))
                        return null;

                    month = GetMonth(1);
                    day = GetDay(1);
                    Years.Where(x => x >= year).First();
                }
                return year;
            }

        }

        public bool IsValidDate(RecurringDateItem dateItem, DateOnly date)
        {
            if (Days.Any() && !Days.Contains(date.Day))
                return false;
            if(Months.Any() && !Months.Contains(date.Month)) 
                return false;
            if (Years.Any() && !Years.Contains(date.Year))
                return false;

            return true;
        }


        public string Serialize()
        {
            string serialised = JsonSerializer.Serialize(this);
            return serialised;
        }
        public IRecurringDateSelector Deserialize(string serialised)
        {
            var deserialised = RecurringDateItem.DeserializeDict(serialised);
            var days = deserialised[nameof(Days)].Replace("[", "").Replace("]", "");
            Days = new();
            if (!string.IsNullOrWhiteSpace(days))
            {
                Days.AddRange(days.Split(",").Select(x => int.Parse(x)));
            }
            var months = deserialised[nameof(Months)].Replace("[", "").Replace("]", "");
            Months = new();
            if (!string.IsNullOrWhiteSpace(months))
            {
                Months.AddRange(months.Split(",").Select(x => int.Parse(x)));
            }
            var years = deserialised[nameof(Years)].Replace("[", "").Replace("]", "");
            Years = new();
            if (!string.IsNullOrWhiteSpace(years))
            {
                Years.AddRange(years.Split(",").Select(x => int.Parse(x)));
            }

            return this;
        }
        #endregion


    }
}
