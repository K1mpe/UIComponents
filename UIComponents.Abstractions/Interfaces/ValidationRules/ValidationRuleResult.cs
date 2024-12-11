using System.Reflection;

namespace UIComponents.Abstractions.Interfaces.ValidationRules;

public class ValidationRuleResult
{
    public bool HasValidationErrors { get; set; }

    public List<ValidationRuleResultError> ValidationErrors { get; set; } = new();

    public ValidationRuleResult AddError(Translatable errorMessage, PropertyInfo? property, Dictionary<string, object> arguments)
    {
        HasValidationErrors = true;
        ValidationErrors.Add(new()
        {
            ErrorMessage = errorMessage,
            Property = property
        });
        return this;
    }

    public ValidationRuleResult ImportErrors(ValidationRuleResult importing)
    {
        foreach(var error in importing.ValidationErrors)
            AddError(error.ErrorMessage, error.Property, error.Arguments);
        return this;
    }

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
    public static ValidationRuleResult HasError(Translatable errorMessage, PropertyInfo? property, Dictionary<string, object> arguments)
    {
        var result = new ValidationRuleResult().AddError(errorMessage, property, arguments);
        return result;
    }

    
}

public class ValidationRuleResultError
{
    public Translatable ErrorMessage { get; set; }
    public PropertyInfo? Property { get; set; }

    /// <summary>
    /// These arguments can be used to parse in the <see cref="ErrorMessage"/>
    /// </summary>
    public Dictionary<string, object> Arguments { get; set; } = new();
}

