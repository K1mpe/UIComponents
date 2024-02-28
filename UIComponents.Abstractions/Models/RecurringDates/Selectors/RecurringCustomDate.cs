using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Models.RecurringDates.Selectors
{
    public class RecurringCustomDate : IRecurringDateSelector
    {
        #region Fields
        public bool IsInvalid => !Days.Any() && !Months.Any()&& !Years.Any();
        public string RenderLocation => "/UIComponents/RecurringDateTypes/RecurringCustom";
        #endregion

        #region Properties
        public List<int> Days { get; set; } = new();
        public List<int> Months { get; set; } = new();
        public List<int> Years { get; set; } = new();

        #endregion


        #region Methods
        public string Serialize()
        {
            throw new NotImplementedException();
        }
        public IRecurringDateSelector Deserialize(string serialised)
        {
            throw new NotImplementedException();
        }

        public DateOnly? GetNextDate(RecurringDateItem dateItem, DateOnly start)
        {
            if (IsValidDate(dateItem, start))
                return start;

            int year = start.Year;
            if (Years.Any())
            {
                if (!Years.Any(x => x >= start.Year))
                    return null;
                year = Years.Where(x=> x>= start.Year).OrderBy(x=>x).FirstOrDefault();
            }
            int month = start.Month;
            if (Months.Any())
            {
                if (!Months.Any(x => x >= start.Month))
                    return null;
                month = Months.Where(x => x >= start.Month).OrderBy(x => x).FirstOrDefault();
            }
            int day = start.Day;
            if (Days.Any())
            {
                if (!Days.Any(x => x >= start.Day))
                    return null;
                day = Days.Where(x => x >= start.Day).OrderBy(x => x).FirstOrDefault();
            }
            

            return null;
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
        #endregion


    }
}
