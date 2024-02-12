namespace UIComponents.Abstractions.Models;


public class RecurringDate
{
    public List<RecurringDateItem> Included { get; set; }
    public List<RecurringDateItem> Excluded { get; set; }

 
    public bool IsValidDate(DateTime date)
    {
        throw new NotImplementedException();
    }
    public DateOnly? GetNextDate(DateTime? startPoint = null)
    {
        return GetNextDates(1,startPoint).FirstOrDefault();
    }
    public List<DateOnly> GetNextDates(int maxCount, DateTime? startPoint = null) 
    {
        throw new NotImplementedException();
    }

    public Translatable GetTranslatable() 
    { 
        throw new NotImplementedException(); 
    }

    public string Serialize()
    {
        throw new NotImplementedException();
    }

    public static RecurringDate Deserialize(string serialized)
    {
        throw new NotImplementedException();
    }
}

public class RecurringDateItem
{
    public bool Enabled { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }

    public IRecurringDateSelector Pattern { get; set; }
}

public class DateSelector : IRecurringDateSelector
{
    public List<int> Days { get; set; } = new();
    public List<int> Months { get; set; } = new();
    public List<int> Years { get; set; } = new();
}

public class DatePattern : IRecurringDateSelector
{
    public RepeatSizeEnum RepeatSize { get; set; }
    public DayOfWeek? DayOfWeek { get; set; }
    public int? Skip { get; set; }

    public enum RepeatSizeEnum
    {
        Day,
        Week,
        Month,
        Year,
    }
}

public interface IRecurringDateSelector
{

}
