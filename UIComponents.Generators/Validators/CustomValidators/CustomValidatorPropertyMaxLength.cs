using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators.CustomValidators;

public class CustomValidatorPropertyMaxLength : IUICPropertyValidationRuleMaxLength
{
    public Func<PropertyInfo, object, Task<ValidationRuleResult>> CheckValidationErrorsFunc { get; set; }
    public Func<PropertyInfo, object, Task<int?>> MaxLengthFunc { get; set; }

    public Type? PropertyType => typeof(object);

    public Task<ValidationRuleResult> CheckValidationErrors(PropertyInfo propertyInfo, object obj)
    {
        return CheckValidationErrorsFunc == null
            ? IUICPropertyValidationRuleMaxLength.DefaultValidationErrors(this, propertyInfo, obj)
            : CheckValidationErrorsFunc(propertyInfo, obj);
    }

    public Task<int?> MaxLength(PropertyInfo propertyInfo, object obj)
    {
        if (MaxLengthFunc == null)
            throw new ArgumentNullException(nameof(MaxLengthFunc));

        return MaxLengthFunc(propertyInfo, obj);
    }
}