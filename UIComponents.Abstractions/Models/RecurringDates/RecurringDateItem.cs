

using System.Text.Json;
using UIComponents.Abstractions.Models.RecurringDates.Selectors;

namespace UIComponents.Abstractions.Models.RecurringDates;

public partial class RecurringDateItem
{
    #region Properties

    public bool Enabled { get; set; } = true;
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? EndDate { get; set; }

    public IRecurringDateSelector Pattern { get; set; }

    
    #endregion

    #region Methods

    /// <summary>
    /// This method checks the start and endDate (if available), but also checks Enabled and if there is a valid Patter available
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    public bool DateInRange(DateOnly date)
    {
        if (Pattern == null)
            return false;
        if (!Enabled)
            return false;
        if (StartDate > date)
            return false;

        if (EndDate != null && EndDate < date)
            return false;

        return true;
    }

    public string Serialize()
    {
        var dict = new Dictionary<string, string>();
        dict[nameof(Enabled)] = Enabled.ToString().ToLower();
        dict[nameof(StartDate)] = $"\"{StartDate.ToString("yyyy-MM-dd")}\"";
        dict[nameof(EndDate)] = EndDate == null ? "null" : $"\"{EndDate.Value.ToString("yyyy-MM-dd")}\"";
        dict[nameof(Pattern)] = Pattern == null ? "null" : Pattern.Serialize();
        dict["PatternType"] = Pattern == null ? "null" : $"\"{Pattern.GetType().Name}\"";


        var serialized = SerializeDict(dict);
        Console.WriteLine(serialized);
        return serialized;

        
    }

    public static string SerializeDict(Dictionary<string, string> dict)
    {
        string output = "{";
        foreach (var x in dict)
        {
            output += $"\"{x.Key}\": {x.Value},";
        }
        //remove the last ,
        output = output.Substring(0, output.Length - 1);
        output += "}";
        return output;
    }
    public static Dictionary<string,string> DeserializeDict(string serialized)
    {
        var dict = new Dictionary<string, string>();
        if (serialized.StartsWith("{"))
            serialized = serialized.Substring(1, serialized.Length - 2);

        var serializedParts = serialized.Split(",");
        for (int i = 0; i < serializedParts.Length; i++)
        {
            try{
                var part = serializedParts[i];
                var key = part.Split(":")[0].Replace("\"", "").Trim();
                var value = part.Split(":")[1].Replace("\"", "").Trim();

                if (value.StartsWith("{") && !value.EndsWith("}"))
                {
                    value = $"{{{part.Split("{")[1].Split("}")[0].Replace("\"", "").Trim()}";
                    int openBrackets = 1;
                    while (i < serializedParts.Length - 1)
                    {
                        i++;
                        var nextPart = serializedParts[i];
                        openBrackets += (nextPart.Split("{").Length - 1);
                        openBrackets -= (nextPart.Split("}").Length - 1);
                        value += $",{nextPart}";
                        if (openBrackets <= 0)
                            break;
                    }
                }
                else if (value.StartsWith("[") && !value.EndsWith("]"))
                {
                    value = $"[{part.Split("[")[1].Split("]")[0].Replace("\"", "").Trim()}";
                    int openBrackets = 1;
                    if (i == serializedParts.Length - 1)
                        value += "]";
                    while (i < serializedParts.Length - 1)
                    {
                        i++;
                        var nextPart = serializedParts[i];
                        openBrackets += (nextPart.Split("[").Length - 1);
                        openBrackets -= (nextPart.Split("]").Length - 1);
                        value += $",{nextPart}";
                        if (openBrackets <= 0)
                            break;
                    }
                }
                dict[key] = value;
            }
            catch(Exception ex)
            {
                var stackTrace = ex.StackTrace;
                throw;
            }
            

        }

        return dict;
    }
    public static RecurringDateItem Deserialize(string serialized)
    {
        var dict = DeserializeDict(serialized);

        var result = new RecurringDateItem();
        result.Enabled = bool.Parse(dict[nameof(Enabled)]);
        result.StartDate = DateOnly.Parse(dict[nameof(StartDate)]);
        if(DateOnly.TryParse(dict[nameof(EndDate)], out var endDate))
            result.EndDate = endDate;

        var type = RecurringDate.GetType(dict["PatternType"]);
        var instance = Activator.CreateInstance(type) as IRecurringDateSelector;
        result.Pattern = instance.Deserialize(dict[nameof(Pattern)]);

        return result;
    }

    
    #endregion

}
