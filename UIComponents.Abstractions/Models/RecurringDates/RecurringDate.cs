using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using UIComponents.Abstractions.Models.RecurringDates.Selectors;
namespace UIComponents.Abstractions.Models.RecurringDates;


public class RecurringDate
{
    public List<RecurringDateItem> Included { get; set; } = new();
    public List<RecurringDateItem> Excluded { get; set; } = new();


    
    #region DateMethods

    /// <summary>
    /// Check if a given date is valid for any <see cref="Included"/> and no <see cref="Excluded"/>
    /// </summary>
    public bool IsValidDate(DateTime date)
    {
        var dateOnly = DateOnly.FromDateTime(date);
        if (Excluded.Where(x => x.DateInRange(dateOnly) && x.Pattern.IsValidDate(x, dateOnly)).Any())
            return false;

        return Included.Where(x => x.DateInRange(dateOnly) && x.Pattern.IsValidDate(x, dateOnly)).Any();

    }

    /// <summary>
    /// Gets the next occuring date. The provided date is included.
    /// </summary>
    /// <param name="startPoint"></param>
    /// <returns></returns>
    public DateOnly? GetNextDate(DateTime? startPoint = null)
    {
        return GetNextDates(1, startPoint).FirstOrDefault();
    }

    /// <summary>
    /// Gets a list off next occurring dates. The provided date is valid as a result.
    /// </summary>
    /// <remarks>
    /// There can be fewer results available then requested.
    /// </remarks>
    public List<DateOnly> GetNextDates(int maxCount, DateTime? startPoint = null)
    {
        DateOnly startDate = DateOnly.FromDateTime(startPoint ?? DateTime.Today);
        List<DateOnly> dates = new();

        List<RecurringDateItem> included = Included.Where(x => x.Enabled && x.Pattern != null).ToList();
        while (dates.Count() < maxCount && included.Any())
        {
            //filter excluded to only be the excludeds that are still valid
            var excluded = Excluded.Where(x => x.Enabled && x.Pattern != null && x.StartDate <= startDate && (x.EndDate == null || x.EndDate.Value >= startDate)).ToList();

            DateOnly? foundDate = null;
            DateOnly? firstExcludedDate = null;
            foreach (var incl in included.ToList())
            {
                //Remove included where enddate is passed
                if (incl.EndDate != null && incl.EndDate < startDate)
                {
                    included.Remove(incl);
                    continue;
                }

                var result = incl.Pattern.GetNextDate(incl, startDate);

                //Remove includeds that do not have a value anymore
                if (result == null)
                {
                    included.Remove(incl);
                    continue;
                }

                if (!incl.DateInRange(result.Value))
                {
                    included.Remove(incl);
                    continue;
                }

                //Check if this date is excluded
                if (excluded.Where(x => x.Pattern.IsValidDate(x, result.Value)).Any())
                {
                    if (firstExcludedDate == null || result.Value < firstExcludedDate.Value)
                        firstExcludedDate = result;
                    continue;
                }

                //If the result value is lower than the already found value, replace this
                if (foundDate == null || result.Value < foundDate.Value)
                    foundDate = result;

            }
            if (foundDate != null)
            {
                dates.Add(foundDate.Value);
            }
            if(foundDate != null || firstExcludedDate != null)
            {
                if(foundDate.HasValue)
                    startDate = foundDate.Value.AddDays(1);
                if (firstExcludedDate.HasValue)
                    if (foundDate == null || firstExcludedDate < foundDate)
                        startDate = firstExcludedDate.Value.AddDays(1);
            }
            else
            {
                break;
            }

        }
        return dates;
    }

    private static List<(string Name, Type Type)> _knownTypes = new(){
        new(nameof(RecurringWeekly), typeof(RecurringWeekly)),
        new(nameof(RecurringMonthly), typeof(RecurringMonthly)),
        new(nameof(RecurringCustomDate), typeof(RecurringCustomDate)),
    };
    public static Type? GetType(string name)
    {
        return _knownTypes.Where(x => x.Name == name).Select(x => x.Type).FirstOrDefault();
    }
    #endregion

    #region Serializing

    /// <summary>
    /// Convert this object to a string that can be stored in a database.
    /// </summary>
    public string Serialize()
    {
        var dict = new Dictionary<string, string>();
        List<string> included = Included.Select(x => x.Serialize()).ToList();
        List<string> excluded = Excluded.Select(x => x.Serialize()).ToList();

        dict[nameof(Included)]=$"[{String.Join(",",included)}]";
        dict[nameof(Excluded)]= $"[{String.Join(",", excluded)}]";

        return RecurringDateItem.SerializeDict(dict);
    }

    /// <summary>
    /// Get the object from a serialized string.
    /// </summary>
    public static RecurringDate Deserialize(string serialized)
    {
        var dict = RecurringDateItem.DeserializeDict(serialized);

        var result = new RecurringDate();

        if (dict[nameof(Included)] != "[]")
        {
            var includedStrings = dict[nameof(Included)].Substring(1, dict[nameof(Included)].Length - 2).Split("},{");
            for (int i = 0; i < includedStrings.Length; i++)
            {
                string value = includedStrings[i];
                if (i > 0)
                    value = $"{{{value}";
                if (i < includedStrings.Length - 1)
                    value = $"{value}}}";
                result.Included.Add(RecurringDateItem.Deserialize(value));
            }
        }
        
        if (dict[nameof(Excluded)] != "[]")
        {
            var excludedStrings = dict[nameof(Excluded)].Substring(1, dict[nameof(Excluded)].Length - 2).Split("},{");
            for (int i = 0; i < excludedStrings.Length; i++)
            {
                string value = excludedStrings[i];
                if (i > 0)
                    value = $"{{{value}";
                if (i < excludedStrings.Length - 1)
                    value = $"{value}}}";
                result.Excluded.Add(RecurringDateItem.Deserialize(value));
            }
        }
        
        return result;
    }
    #endregion
}


