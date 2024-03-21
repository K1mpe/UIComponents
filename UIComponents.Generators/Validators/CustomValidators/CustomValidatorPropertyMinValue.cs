using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators.CustomValidators;

public class CustomValidatorPropertyMinValue<TValue> : IUICPropertyValidationRuleMinValue<TValue> where TValue : struct, IComparable
{
    public Func<PropertyInfo, object, Task<ValidationRuleResult>> CheckValidationErrorsFunc { get; set; }
    public Func<PropertyInfo, object, Task<Nullable<TValue>>> MinValueFunc { get; set; }

    public Type? PropertyType => typeof(object);

    public async Task<ValidationRuleResult> CheckValidationErrors(PropertyInfo propertyInfo, object obj)
    {
        if (CheckValidationErrorsFunc == null)
        {
            var value = await MinValue(propertyInfo, obj);
            if (value == null)
                return ValidationRuleResult.IsValid();

            var propValue = propertyInfo.GetValue(obj);
            if (propValue == null)
                return ValidationRuleResult.IsValid();

            var convertedVal = (TValue)propValue;
            if (convertedVal.CompareTo(value) > -1)
                return ValidationRuleResult.IsValid();

            var property = TranslationDefaults.TranslateProperty(propertyInfo, null);
            var message = TranslationDefaults.ValidateMaxValue(property, value.Value);
            return ValidationRuleResult.HasError(message, propertyInfo);
        }

        return await CheckValidationErrorsFunc(propertyInfo, obj);
    }

    public Task<Nullable<TValue>> MinValue(PropertyInfo propertyInfo, object obj)
    {
        if (MinValueFunc == null)
            throw new ArgumentNullException(nameof(MinValueFunc));

        return MinValueFunc(propertyInfo, obj);
    }

}