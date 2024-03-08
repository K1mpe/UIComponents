namespace UIComponents.Abstractions.Interfaces.ValidationRules;

public class ValidationRuleResult
{
    public bool HasValidationErrors { get; set; }

    public List<Translatable> ValidationErrors { get; set; }


    /// <summary>
    /// Result that validation rule does not contain any validation errors
    /// </summary>
    /// <returns></returns>
    public static ValidationRuleResult IsValid()
    {
        return new();
    }

    /// <summary>
    /// Result that contains a validation error
    /// </summary>
    public static ValidationRuleResult HasError(Translatable errorMessage)
    {
        return HasErrors(new List<Translatable>() { errorMessage });
    }

    /// <summary>
    /// Result that contains multiple validation errors
    /// </summary>
    public static ValidationRuleResult HasErrors(IEnumerable<Translatable> errors)
    {
        return new()
        {
            HasValidationErrors = true,
            ValidationErrors = errors.ToList()
        };
    }
}

