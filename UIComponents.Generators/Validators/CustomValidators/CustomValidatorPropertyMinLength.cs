using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators.CustomValidators;

public class CustomValidatorPropertyMinLength : IUICPropertyValidationRuleMinLength
{
    public Func<PropertyInfo, object, Task<ValidationRuleResult>> CheckValidationErrorsFunc { get; set; }
    public Func<PropertyInfo, object, Task<int?>> MinLengthFunc { get; set; }

    public Type? PropertyType => typeof(object);

    public Task<ValidationRuleResult> CheckValidationErrors(PropertyInfo propertyInfo, object obj)
    {
        return CheckValidationErrorsFunc == null
            ? IUICPropertyValidationRuleMinLength.DefaultValidationErrors(this, propertyInfo, obj)
            : CheckValidationErrorsFunc(propertyInfo, obj);
    }

    public Task<int?> MinLength(PropertyInfo propertyInfo, object obj)
    {
        if (MinLengthFunc == null)
            throw new ArgumentNullException(nameof(MinLengthFunc));

        return MinLengthFunc(propertyInfo, obj);
    }
}