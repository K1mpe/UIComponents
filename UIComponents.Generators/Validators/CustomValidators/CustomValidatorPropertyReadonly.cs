using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators.CustomValidators;

public class CustomValidatorPropertyReadonly : IUICPropertyValidationRuleReadonly
{
    public Func<PropertyInfo, object, Task<ValidationRuleResult>> CheckValidationErrorsFunc { get; set; }
    public Func<PropertyInfo, object, Task<bool>> IsReadonlyFunc { get; set; }

    public Type? PropertyType => typeof(object);

    public Task<ValidationRuleResult> CheckValidationErrors(PropertyInfo propertyInfo, object obj)
    {
        if (CheckValidationErrorsFunc == null)
            throw new ArgumentNullException(nameof(CheckValidationErrorsFunc));

        return CheckValidationErrorsFunc(propertyInfo, obj);
    }

    public Task<bool> IsReadonly(PropertyInfo propertyInfo, object obj)
    {
        if (IsReadonlyFunc == null)
            throw new ArgumentNullException(nameof(IsReadonlyFunc));

        return IsReadonlyFunc(propertyInfo, obj);
    }

}