using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators.CustomValidators;

public class CustomValidatorPropertyRequired : IUICPropertyValidationRuleRequired
{


    public Func<PropertyInfo,object,Task<ValidationRuleResult>> CheckValidationErrorsFunc { get; set; }
    public Func<PropertyInfo,object,Task<bool>> IsRequiredFunc { get; set; }

    public Type? PropertyType => typeof(object);

    public Task<ValidationRuleResult> CheckValidationErrors(PropertyInfo propertyInfo, object obj)
    {
        return CheckValidationErrorsFunc == null 
            ? IUICPropertyValidationRuleRequired.DefaultValidationErrors(this, propertyInfo, obj)
            :CheckValidationErrorsFunc(propertyInfo, obj);
    }

    public Task<bool> IsRequired(PropertyInfo propertyInfo, object obj)
    {
        if (IsRequiredFunc == null)
            throw new ArgumentNullException(nameof(IsRequiredFunc));

        return IsRequiredFunc(propertyInfo, obj);
    }
}
