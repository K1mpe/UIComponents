using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators.CustomValidators;

public class CustomValidatorPropertyMinValue<TValue> : IUICPropertyValidationRuleMinValue<TValue> where TValue : struct, IComparable
{
    public Func<PropertyInfo, object, Task<ValidationRuleResult>> CheckValidationErrorsFunc { get; set; }
    public Func<PropertyInfo, object, Task<Nullable<TValue>>> MinValueFunc { get; set; }

    public Type? PropertyType => typeof(object);

    public Task<ValidationRuleResult> CheckValidationErrors(PropertyInfo propertyInfo, object obj)
    {
        
        return CheckValidationErrorsFunc == null
            ? IUICPropertyValidationRuleMinValue<TValue>.DefaultValidationErrors(this, propertyInfo, obj)
            : CheckValidationErrorsFunc(propertyInfo, obj);
    }

    public Task<Nullable<TValue>> MinValue(PropertyInfo propertyInfo, object obj)
    {
        if (MinValueFunc == null)
            throw new ArgumentNullException(nameof(MinValueFunc));

        return MinValueFunc(propertyInfo, obj);
    }

}