using UIComponents.Abstractions.Interfaces.ValidationRules;

namespace UIComponents.Generators.Validators.CustomValidators;

public class CustomValidatorPropertyMinLength : IUICPropertyValidationRuleMinLength
{
    public Func<PropertyInfo, object, Task<ValidationRuleResult>> CheckValidationErrorsFunc { get; set; }
    public Func<PropertyInfo, object, Task<int?>> MinLengthFunc { get; set; }

    public Type? PropertyType => typeof(object);

    public async Task<ValidationRuleResult> CheckValidationErrors(PropertyInfo propertyInfo, object obj)
    {
        if (CheckValidationErrorsFunc == null)
        {
            int? minLength = await MinLength(propertyInfo, obj);
            if (minLength == null)
                return ValidationRuleResult.IsValid();

            var value = propertyInfo.GetValue(obj)?.ToString() ?? string.Empty;
            if (value.Length >= minLength)
                return ValidationRuleResult.IsValid();

            var translatedProp = TranslationDefaults.TranslateProperty(propertyInfo, null);
            var message = TranslationDefaults.ValidateMinLength(translatedProp, minLength.Value);
            return ValidationRuleResult.HasError(message, propertyInfo);
        }

        return await CheckValidationErrorsFunc(propertyInfo, obj);
    }

    public Task<int?> MinLength(PropertyInfo propertyInfo, object obj)
    {
        if (MinLengthFunc == null)
            throw new ArgumentNullException(nameof(MinLengthFunc));

        return MinLengthFunc(propertyInfo, obj);
    }
}