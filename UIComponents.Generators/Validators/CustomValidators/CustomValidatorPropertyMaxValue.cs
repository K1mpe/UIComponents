using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators.CustomValidators;

public class CustomValidatorPropertyMaxValue<TValue> : IUICPropertyValidationRuleMaxValue<TValue> where TValue : struct, IComparable
{


    public Func<PropertyInfo, object, Task<ValidationRuleResult>> CheckValidationErrorsFunc { get; set; }
    public Func<PropertyInfo, object, Task<Nullable<TValue>>> MaxValueFunc { get; set; }

    public Type? PropertyType => typeof(object);

    public async Task<ValidationRuleResult> CheckValidationErrors(PropertyInfo propertyInfo, object obj)
    {
        if (CheckValidationErrorsFunc == null)
        {
            var value = await MaxValue(propertyInfo, obj);
            if (value == null)
                return ValidationRuleResult.IsValid();

            var propValue = propertyInfo.GetValue(obj);
            if(propValue == null)
                return ValidationRuleResult.IsValid();

            var convertedVal = (TValue)propValue;
            if (convertedVal.CompareTo(value) < 1)
                return ValidationRuleResult.IsValid();

            var property = TranslationDefaults.TranslateProperty(propertyInfo, null);
            var message = TranslationDefaults.ValidateMaxValue(property, value.Value);
            return ValidationRuleResult.HasError(message, propertyInfo);
        }

        return await CheckValidationErrorsFunc(propertyInfo, obj);
    }

    public Task<Nullable<TValue>> MaxValue(PropertyInfo propertyInfo, object obj)
    {
        if (MaxValueFunc == null)
            throw new ArgumentNullException(nameof(MaxValueFunc));

        return MaxValueFunc(propertyInfo, obj);
    }

}