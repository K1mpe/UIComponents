using System;
using System.Reflection;
using UIComponents.Defaults;

namespace UIComponents.Abstractions.Interfaces.ValidationRules;

public interface IUICPropertyValidationRule
{
    public Type? PropertyType { get; }
    public Task<ValidationRuleResult> CheckValidationErrors(PropertyInfo propertyInfo, object obj);
}

public interface IUICPropertyValidationRule<T> : IUICPropertyValidationRule
{
    public Type PropertyType => typeof(T);
    public Task<ValidationRuleResult> CheckValidationErrors(PropertyInfo propertyInfo, object obj);
}

public interface IUICPropertyValidationRuleRequired : IUICPropertyValidationRule
{
    public Task<bool> IsRequired(PropertyInfo propertyInfo, object obj);

    public static Func<IUICPropertyValidationRuleRequired, PropertyInfo, object, Task<ValidationRuleResult>> DefaultValidationErrors { get; set; } = async (validator, propertyInfo, obj) =>
    {
        if (!await validator.IsRequired(propertyInfo, obj))
            return ValidationRuleResult.IsValid();

        var value = propertyInfo.GetValue(obj);
        if (value != default)
            return ValidationRuleResult.IsValid();

        var translatedProp = TranslationDefaults.TranslateProperty(propertyInfo, null);
        var message = TranslationDefaults.ValidationIsRequired(translatedProp);

        return ValidationRuleResult.HasError(message, propertyInfo);
    };
}

public interface IUICPropertyValidationRuleMinValue<TValueType> : IUICPropertyValidationRule<TValueType> where TValueType : struct, IComparable
{
    Task<Nullable<TValueType>> MinValue(PropertyInfo propertyInfo, object obj);


    public static Func<IUICPropertyValidationRuleMinValue<TValueType>, PropertyInfo, object, Task<ValidationRuleResult>> DefaultValidationErrors { get; set; } = async (validator, propertyInfo, obj) =>
    {
        var value = await validator.MinValue(propertyInfo, obj);
        if (value == null)
            return ValidationRuleResult.IsValid();

        var propValue = propertyInfo.GetValue(obj);
        if (propValue == null)
            return ValidationRuleResult.IsValid();

        var convertedVal = (TValueType)propValue;
        if (convertedVal.CompareTo(value) > -1)
            return ValidationRuleResult.IsValid();

        var property = TranslationDefaults.TranslateProperty(propertyInfo, null);
        var message = TranslationDefaults.ValidateMinValue(property, value.Value);
        return ValidationRuleResult.HasError(message, propertyInfo);
    };
}

public interface IUICPropertyValidationRuleMaxValue<TValueType> : IUICPropertyValidationRule<TValueType> where TValueType : struct, IComparable
{
    Task<Nullable<TValueType>> MaxValue(PropertyInfo propertyInfo, object obj);

    public static Func<IUICPropertyValidationRuleMaxValue<TValueType>, PropertyInfo, object, Task<ValidationRuleResult>> DefaultValidationErrors { get; set; } = async (validator, propertyInfo, obj) =>
    {
        var value = await validator.MaxValue(propertyInfo, obj);
        if (value == null)
            return ValidationRuleResult.IsValid();

        var propValue = propertyInfo.GetValue(obj);
        if (propValue == null)
            return ValidationRuleResult.IsValid();

        var convertedVal = (TValueType)propValue;
        if (convertedVal.CompareTo(value) < 1)
            return ValidationRuleResult.IsValid();

        var property = TranslationDefaults.TranslateProperty(propertyInfo, null);
        var message = TranslationDefaults.ValidateMaxValue(property, value.Value);
        return ValidationRuleResult.HasError(message, propertyInfo);
    };
}

public interface IUICPropertyValidationRuleMinLength : IUICPropertyValidationRule<string>
{
    Task<int?> MinLength(PropertyInfo propertyInfo, object obj);

    public static Func<IUICPropertyValidationRuleMinLength, PropertyInfo, object, Task<ValidationRuleResult>> DefaultValidationErrors { get; set; } = async (validator, propertyInfo, obj) =>
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
    };
}

public interface IUICPropertyValidationRuleMaxLength : IUICPropertyValidationRule<string>
{
    Task<int?> MaxLength(PropertyInfo propertyInfo, object obj);

    public static Func<IUICPropertyValidationRuleMaxLength, PropertyInfo, object, Task<ValidationRuleResult>> DefaultValidationErrors { get; set; } = async (validator, propertyInfo, obj) =>
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
    };
}

public interface IUICPropertyValidationRuleReadonly : IUICPropertyValidationRule
{
    Task<bool> IsReadonly(PropertyInfo propertyInfo, object obj);
}