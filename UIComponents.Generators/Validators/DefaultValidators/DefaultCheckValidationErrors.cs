using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Generators.Configuration;

namespace UIComponents.Generators.Validators.DefaultValidators;

public class DefaultCheckValidationErrorsRequired : IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleRequired>
{
    public async Task<ValidationRuleResult> DefaultValidationErrors(IUICPropertyValidationRuleRequired validator, PropertyInfo propertyInfo, object obj)
    {
        if (!await validator.IsRequired(propertyInfo, obj))
            return ValidationRuleResult.IsValid();

        var value = propertyInfo.GetValue(obj);
        object defaultValue = null;
        if (propertyInfo.PropertyType.IsValueType)
            defaultValue = Activator.CreateInstance(propertyInfo.PropertyType);

        if (value != defaultValue)
            return ValidationRuleResult.IsValid();

        var translatedProp = TranslationDefaults.TranslateProperty(propertyInfo, null);
        var message = TranslationDefaults.ValidationIsRequired(translatedProp);

        return ValidationRuleResult.HasError(message, propertyInfo);
    }
}

public class DefaultCheckValidationErrorsMinValue<TValue> : IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<TValue>> where TValue : struct, IComparable
{
    public async Task<ValidationRuleResult> DefaultValidationErrors(IUICPropertyValidationRuleMinValue<TValue> validator, PropertyInfo propertyInfo, object obj)
    {
        var type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
        if (!type.IsAssignableTo(typeof(TValue)))
            return ValidationRuleResult.IsValid();

        var value = await validator.MinValue(propertyInfo, obj);
        if (value == null)
            return ValidationRuleResult.IsValid();

        var propValue = propertyInfo.GetValue(obj);
        if (propValue == null)
            return ValidationRuleResult.IsValid();

        var convertedVal = (TValue)propValue;
        if (convertedVal.CompareTo(value) > -1)
            return ValidationRuleResult.IsValid();

        var property = TranslationDefaults.TranslateProperty(propertyInfo, null);
        var message = TranslationDefaults.ValidateMinValue(property, value.Value);
        return ValidationRuleResult.HasError(message, propertyInfo);
    }
}

public class DefaultCheckValidationErrorsMaxValue<TValue> : IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<TValue>> where TValue : struct, IComparable
{
    public async Task<ValidationRuleResult> DefaultValidationErrors(IUICPropertyValidationRuleMaxValue<TValue> validator, PropertyInfo propertyInfo, object obj)
    {
        var type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
        if (!type.IsAssignableTo(typeof(TValue)))
            return ValidationRuleResult.IsValid();

        var value = await validator.MaxValue(propertyInfo, obj);
        if (value == null)
            return ValidationRuleResult.IsValid();

        var propValue = propertyInfo.GetValue(obj);
        if (propValue == null)
            return ValidationRuleResult.IsValid();

        var convertedVal = (TValue)propValue;
        if (convertedVal.CompareTo(value) < 1)
            return ValidationRuleResult.IsValid();

        var property = TranslationDefaults.TranslateProperty(propertyInfo, null);
        var message = TranslationDefaults.ValidateMaxValue(property, value.Value);
        return ValidationRuleResult.HasError(message, propertyInfo);
    }
}

public class DefaultCheckValidationErrorsMinLength : IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinLength>
{
    public async Task<ValidationRuleResult> DefaultValidationErrors(IUICPropertyValidationRuleMinLength validator, PropertyInfo propertyInfo, object obj)
    {
        int? minLength = await validator.MinLength(propertyInfo, obj);
        if (minLength == null)
            return ValidationRuleResult.IsValid();

        var value = propertyInfo.GetValue(obj)?.ToString() ?? string.Empty;
        if (value.Length >= minLength)
            return ValidationRuleResult.IsValid();

        var translatedProp = TranslationDefaults.TranslateProperty(propertyInfo, null);
        var message = TranslationDefaults.ValidateMinLength(translatedProp, minLength.Value);
        return ValidationRuleResult.HasError(message, propertyInfo);
    }
}
public class DefaultCheckValidationErrorsMaxLength : IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxLength>
{
    public async Task<ValidationRuleResult> DefaultValidationErrors(IUICPropertyValidationRuleMaxLength validator, PropertyInfo propertyInfo, object obj)
    {
        int? maxLength = await validator.MaxLength(propertyInfo, obj);
        if (maxLength == null)
            return ValidationRuleResult.IsValid();

        var value = propertyInfo.GetValue(obj)?.ToString() ?? string.Empty;
        if (value.Length <= maxLength)
            return ValidationRuleResult.IsValid();

        var translatedProp = TranslationDefaults.TranslateProperty(propertyInfo, null);
        var message = TranslationDefaults.ValidateMaxLength(translatedProp, maxLength.Value);
        return ValidationRuleResult.HasError(message, propertyInfo);
    }
}

public class DefaultCheckValidationErrorsReadonly : IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleReadonly>
{
    private readonly ILogger _logger;
    private readonly UicConfigOptions _uicConfigOptions;
    public DefaultCheckValidationErrorsReadonly(ILogger<DefaultCheckValidationErrorsReadonly> logger, UicConfigOptions uicConfigOptions)
    {
        _logger = logger;
        _uicConfigOptions = uicConfigOptions;
    }

    private static bool HasChecked { get; set; }

    public Task<ValidationRuleResult> DefaultValidationErrors(IUICPropertyValidationRuleReadonly validator, PropertyInfo propertyInfo, object obj)
    {
        if (!_uicConfigOptions.CheckPropertyValidatorReadonly)
        {
            _logger.LogTrace($"{nameof(DefaultCheckValidationErrorsReadonly)} is disabled in the uicConfig.");
            return Task.FromResult(ValidationRuleResult.IsValid());
        }
           
        if (HasChecked)
        {
            _logger.LogDebug($"{nameof(DefaultCheckValidationErrorsReadonly)} has already thrown a error this build");
            return Task.FromResult(ValidationRuleResult.IsValid());
        }

        HasChecked = true;
        _logger.LogError($"There is no valid implementation registrated for {nameof(IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleReadonly>)}. Please create implementation or disable this in the config options => options.{nameof(UicConfigOptions.CheckPropertyValidatorReadonly)} = false");
        return Task.FromResult(ValidationRuleResult.IsValid());
    }
}