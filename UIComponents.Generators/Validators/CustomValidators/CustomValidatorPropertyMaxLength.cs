using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators.CustomValidators;

public class CustomValidatorPropertyMaxLength : IUICPropertyValidationRuleMaxLength
{
    public Func<PropertyInfo, object, Task<ValidationRuleResult>> CheckValidationErrorsFunc { get; set; }
    public Func<PropertyInfo, object, Task<int?>> MaxLengthFunc { get; set; }

    public Type? PropertyType => typeof(object);

    public async Task<ValidationRuleResult> CheckValidationErrors(PropertyInfo propertyInfo, object obj)
    {
        if (CheckValidationErrorsFunc == null)
        {
            int? maxLength = await MaxLength(propertyInfo, obj);
            if (maxLength == null)
                return ValidationRuleResult.IsValid();

            var value = propertyInfo.GetValue(obj)?.ToString()??string.Empty;
            if(value.Length <= maxLength)
                return ValidationRuleResult.IsValid();

            var translatedProp = TranslationDefaults.TranslateProperty(propertyInfo, null);
            var message = TranslationDefaults.ValidateMaxLength(translatedProp, maxLength.Value);
            return ValidationRuleResult.HasError(message, propertyInfo);
        }


        return await CheckValidationErrorsFunc(propertyInfo, obj);
    }

    public Task<int?> MaxLength(PropertyInfo propertyInfo, object obj)
    {
        if (MaxLengthFunc == null)
            throw new ArgumentNullException(nameof(MaxLengthFunc));

        return MaxLengthFunc(propertyInfo, obj);
    }
}