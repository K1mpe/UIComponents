using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Generators.Configuration;

namespace UIComponents.Generators.Services;

public class UICValidationService : IUICValidationService
{
    #region Fields
    private readonly ILogger _logger;
    private readonly UicConfigOptions _config;
    private readonly IServiceProvider _serviceProvider;

    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleRequired> _defaultRequired;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleReadonly> _defaultReadonly;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinLength> _defaultMinLength;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxLength> _defaultMaxLength;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<short>> _defaultshortMinValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<short>> _defaultshortMaxValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<int>> _defaultintMinValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<int>> _defaultintMaxValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<long>> _defaultlongMinValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<long>> _defaultlongMaxValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<float>> _defaultfloatMinValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<float>> _defaultfloatMaxValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<double>> _defaultdoubleMinValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<double>> _defaultdoubleMaxValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<decimal>> _defaultdecimalMinValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<decimal>> _defaultdecimalMaxValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<DateTime>> _defaultDateTimeMinValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<DateTime>> _defaultDateTimeMaxValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<DateOnly>> _defaultDateOnlyMinValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<DateOnly>> _defaultDateOnlyMaxValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<TimeOnly>> _defaultTimeOnlyMinValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<TimeOnly>> _defaultTimeOnlyMaxValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<TimeSpan>> _defaultTimespanMinValue;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<TimeSpan>> _defaultTimespanMaxValue;
    #endregion

    #region Ctor
    public UICValidationService(
        ILogger<UICValidationService> logger,
        UicConfigOptions config,
        IServiceProvider serviceProvider,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleRequired> defaultRequired,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleReadonly> defaultReadonly,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinLength> defaultMinLength,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxLength> defaultMaxLength,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<short>> defaultshortMinValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<short>> defaultshortMaxValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<int>> defaultintMinValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<int>> defaultintMaxValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<long>> defaultlongMinValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<long>> defaultlongMaxValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<float>> defaultfloatMinValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<float>> defaultfloatMaxValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<double>> defaultdoubleMinValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<double>> defaultdoubleMaxValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<decimal>> defaultdecimalMinValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<decimal>> defaultdecimalMaxValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<DateTime>> defaultDateTimeMinValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<DateTime>> defaultDateTimeMaxValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<DateOnly>> defaultDateOnlyMinValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<DateOnly>> defaultDateOnlyMaxValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<TimeOnly>> defaultTimeOnlyMinValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<TimeOnly>> defaultTimeOnlyMaxValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMinValue<TimeSpan>> defaultTimespanMinValue,
        IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleMaxValue<TimeSpan>> defaultTimespanMaxValue
        )
    {
        _logger = logger;
        _config = config;
        _serviceProvider = serviceProvider;
        _defaultRequired = defaultRequired;
        _defaultReadonly = defaultReadonly;
        _defaultMinLength = defaultMinLength;
        _defaultMaxLength = defaultMaxLength;
        _defaultshortMinValue = defaultshortMinValue;
        _defaultshortMaxValue = defaultshortMaxValue;
        _defaultintMinValue = defaultintMinValue;
        _defaultintMaxValue = defaultintMaxValue;
        _defaultlongMinValue = defaultlongMinValue;
        _defaultlongMaxValue = defaultlongMaxValue;
        _defaultfloatMinValue = defaultfloatMinValue;
        _defaultfloatMaxValue = defaultfloatMaxValue;
        _defaultdoubleMinValue = defaultdoubleMinValue;
        _defaultdoubleMaxValue = defaultdoubleMaxValue;
        _defaultdecimalMinValue = defaultdecimalMinValue;
        _defaultdecimalMaxValue = defaultdecimalMaxValue;
        _defaultDateTimeMinValue = defaultDateTimeMinValue;
        _defaultDateTimeMaxValue = defaultDateTimeMaxValue;
        _defaultDateOnlyMinValue = defaultDateOnlyMinValue;
        _defaultDateOnlyMaxValue = defaultDateOnlyMaxValue;
        _defaultTimeOnlyMinValue = defaultTimeOnlyMinValue;
        _defaultTimeOnlyMaxValue = defaultTimeOnlyMaxValue;
        _defaultTimespanMinValue = defaultTimespanMinValue;
        _defaultTimespanMaxValue = defaultTimespanMaxValue;
    }
    #endregion

    #region Interface implementation

    public async Task<ValidationRuleResult> ValidateObjectAsync(object obj)
    {
        if(obj == null)
            throw new ArgumentNullException(nameof(obj));
        
        var result = new ValidationRuleResult();

        foreach (var property in obj.GetType().GetProperties())
        {
            try
            {
                var propValidatorResult = await ValidateObjectProperty(property, obj);
                foreach (var error in propValidatorResult.ValidationErrors)
                    result.AddError(error.ErrorMessage, property ?? property);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in validating {0}", $"{property.DeclaringType.Name} => {property.Name}");
            }
        }

        return result;
    }

    public async Task<ValidationRuleResult> ValidateObjectProperty(PropertyInfo propertyInfo, object obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));
        if(propertyInfo == null)
            throw new ArgumentNullException(nameof(obj));

        ValidationRuleResult result = new();
        using (var scope = _serviceProvider.CreateScope())
        {
            var validators = _config.GetPropertyValidators(_logger, scope);
            foreach (var validator in validators)
            {
                _logger.LogTrace("Validating {0} with validator: {1}", $"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name}", validator.GetType().FullName);
                try
                {
                    var propType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                    if (validator.PropertyType != null && !propType.IsAssignableTo(validator.PropertyType))
                        continue;

                    ValidationRuleResult validatorResult = null;
                    if (validator is IUICPropertyValidationValidationResultsImplementation implementation)
                        validatorResult = await implementation.CheckValidationErrors(propertyInfo, obj);
                    else
                    {
                        //Find the default validator
                        if (validator is IUICPropertyValidationRuleRequired required)
                            validatorResult = await _defaultRequired.DefaultValidationErrors(required, propertyInfo, obj);

                        else if (validator is IUICPropertyValidationRuleReadonly readOnly)
                            validatorResult = await _defaultReadonly.DefaultValidationErrors(readOnly, propertyInfo, obj);

                        else if (validator is IUICPropertyValidationRuleMinLength minLength)
                            validatorResult = await _defaultMinLength.DefaultValidationErrors(minLength, propertyInfo, obj);

                        else if (validator is IUICPropertyValidationRuleMaxLength maxLength)
                            validatorResult = await _defaultMaxLength.DefaultValidationErrors(maxLength, propertyInfo, obj);

                        else if (validator is IUICPropertyValidationRuleMinValue<short> minValueShort)
                            validatorResult = await _defaultshortMinValue.DefaultValidationErrors(minValueShort, propertyInfo, obj);

                        else if (validator is IUICPropertyValidationRuleMaxValue<short> maxValueShort)
                            validatorResult = await _defaultshortMaxValue.DefaultValidationErrors(maxValueShort, propertyInfo, obj);

                        else if (validator is IUICPropertyValidationRuleMinValue<int> minValueInt)
                            validatorResult = await _defaultintMinValue.DefaultValidationErrors(minValueInt, propertyInfo, obj);

                        else if (validator is IUICPropertyValidationRuleMaxValue<int> maxValueInt)
                            validatorResult = await _defaultintMaxValue.DefaultValidationErrors(maxValueInt, propertyInfo, obj);

                        else if (validator is IUICPropertyValidationRuleMinValue<long> minValueLong)
                            validatorResult = await _defaultlongMinValue.DefaultValidationErrors(minValueLong, propertyInfo, obj);
                        else if (validator is IUICPropertyValidationRuleMaxValue<long> maxValueLong)
                            validatorResult = await _defaultlongMaxValue.DefaultValidationErrors(maxValueLong, propertyInfo, obj);

                        else if (validator is IUICPropertyValidationRuleMinValue<float> minValueFloat)
                            validatorResult = await _defaultfloatMinValue.DefaultValidationErrors(minValueFloat, propertyInfo, obj);
                        else if (validator is IUICPropertyValidationRuleMaxValue<float> maxValueFloat)
                            validatorResult = await _defaultfloatMaxValue.DefaultValidationErrors(maxValueFloat, propertyInfo, obj);

                        else if (validator is IUICPropertyValidationRuleMinValue<double> minValueDouble)
                            validatorResult = await _defaultdoubleMinValue.DefaultValidationErrors(minValueDouble, propertyInfo, obj);
                        else if (validator is IUICPropertyValidationRuleMaxValue<double> maxValueDouble)
                            validatorResult = await _defaultdoubleMaxValue.DefaultValidationErrors(maxValueDouble, propertyInfo, obj);

                        else if (validator is IUICPropertyValidationRuleMinValue<decimal> minValueDecimal)
                            validatorResult = await _defaultdecimalMinValue.DefaultValidationErrors(minValueDecimal, propertyInfo, obj);
                        else if (validator is IUICPropertyValidationRuleMaxValue<decimal> maxValueDecimal)
                            validatorResult = await _defaultdecimalMaxValue.DefaultValidationErrors(maxValueDecimal, propertyInfo, obj);

                        else if (validator is IUICPropertyValidationRuleMinValue<DateTime> minValueDateTime)
                            validatorResult = await _defaultDateTimeMinValue.DefaultValidationErrors(minValueDateTime, propertyInfo, obj);
                        else if (validator is IUICPropertyValidationRuleMaxValue<DateTime> maxValueDateTime)
                            validatorResult = await _defaultDateTimeMaxValue.DefaultValidationErrors(maxValueDateTime, propertyInfo, obj);

                        else if (validator is IUICPropertyValidationRuleMinValue<DateOnly> minValueDateOnly)
                            validatorResult = await _defaultDateOnlyMinValue.DefaultValidationErrors(minValueDateOnly, propertyInfo, obj);
                        else if (validator is IUICPropertyValidationRuleMaxValue<DateOnly> maxValueDateOnly)
                            validatorResult = await _defaultDateOnlyMaxValue.DefaultValidationErrors(maxValueDateOnly, propertyInfo, obj);

                        else if (validator is IUICPropertyValidationRuleMinValue<TimeOnly> minValueTimeOnly)
                            validatorResult = await _defaultTimeOnlyMinValue.DefaultValidationErrors(minValueTimeOnly, propertyInfo, obj);
                        else if (validator is IUICPropertyValidationRuleMaxValue<TimeOnly> maxValueTimeOnly)
                            validatorResult = await _defaultTimeOnlyMaxValue.DefaultValidationErrors(maxValueTimeOnly, propertyInfo, obj);

                        else if (validator is IUICPropertyValidationRuleMinValue<TimeSpan> minValueTimeSpan)
                            validatorResult = await _defaultTimespanMinValue.DefaultValidationErrors(minValueTimeSpan, propertyInfo, obj);
                        else if (validator is IUICPropertyValidationRuleMaxValue<TimeSpan> maxValueTimeSpan)
                            validatorResult = await _defaultTimespanMaxValue.DefaultValidationErrors(maxValueTimeSpan, propertyInfo, obj);

                        else
                            throw new Exception($"There is no ValidationErrors method available for {validator.GetType().FullName}. Add the {nameof(IUICPropertyValidationValidationResultsImplementation)} interface to fix this problem.");
                    }


                    foreach (var error in validatorResult.ValidationErrors)
                    {
                        _logger.LogDebug($"ValidationError for {propertyInfo.DeclaringType.Name} => {propertyInfo.Name}: {error.ErrorMessage}");
                        result.AddError(error.ErrorMessage, error.Property ?? propertyInfo);
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception in validating {0}", validator.GetType().FullName);
                }

            }
            return result;
        }
    }

    public async Task<int?> ValidatePropertyMaxLength(PropertyInfo propertyInfo, object obj)
    {
        if (!propertyInfo.PropertyType.IsAssignableTo(typeof(string)))
            return null;
        using (var scope = _serviceProvider.CreateScope())
        {
            var validators = _config.GetPropertyValidators(_logger, scope);
            int? maxLength = null;
            foreach (var validator in validators)
            {
                try
                {
                    if (validator is IUICPropertyValidationRuleMaxLength maxLengthValidator)
                    {
                        var result = await maxLengthValidator.MaxLength(propertyInfo, obj);
                        if (result == null)
                            continue;
                        if (maxLength == null || result < maxLength)
                            maxLength = result;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception when validating max length of {0}",  $"{obj.GetType().Name} => {propertyInfo.Name}");
                }
            }
            return maxLength;
        }
    }

    public async Task<Nullable<TValueType>> ValidatePropertyMaxValue<TValueType>(PropertyInfo propertyInfo, object obj) where TValueType : struct, IComparable
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var validators = _config.GetPropertyValidators(_logger, scope);
            Nullable<TValueType> maxValue = null;
            foreach (var validator in validators)
            {
                try
                {
                    if (validator is IUICPropertyValidationRuleMaxValue<TValueType> maxLengthValidator)
                    {
                        var result = await maxLengthValidator.MaxValue(propertyInfo, obj);
                        if (result == null)
                            continue;
                        if (maxValue == null || result.Value.CompareTo(maxValue.Value) < 0)
                            maxValue = result;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception when validating max value of {0}",$"{obj.GetType().Name} => {propertyInfo.Name}");
                }
            }
            return maxValue;
        }
    }

    public async Task<int?> ValidatePropertyMinLength(PropertyInfo propertyInfo, object obj)
    {
        if (!propertyInfo.PropertyType.IsAssignableTo(typeof(string)))
            return null;
        using (var scope = _serviceProvider.CreateScope())
        {
            var validators = _config.GetPropertyValidators(_logger, scope);
            int? maxLength = null;
            foreach (var validator in validators)
            {
                try
                {
                    if (validator is IUICPropertyValidationRuleMinLength minLengthValidator)
                    {
                        var result = await minLengthValidator.MinLength(propertyInfo, obj);
                        if (result == null)
                            continue;
                        if (maxLength == null || result > maxLength)
                            maxLength = result;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception when validating min length of {0}", $"{obj.GetType().Name} => {propertyInfo.Name}");
                }
            }
            return maxLength;
        }
    }

    public async Task<Nullable<TValueType>> ValidatePropertyMinValue<TValueType>(PropertyInfo propertyInfo, object obj) where TValueType : struct, IComparable
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var validators = _config.GetPropertyValidators(_logger, scope);
            Nullable<TValueType> maxValue = null;
            foreach (var validator in validators)
            {
                try
                {
                    if (validator is IUICPropertyValidationRuleMinValue<TValueType> minLengthValidator)
                    {
                        var result = await minLengthValidator.MinValue(propertyInfo, obj);
                        if (result == null)
                            continue;
                        if (maxValue == null || result.Value.CompareTo(maxValue.Value) > 0)
                            maxValue = result;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception when validating max length of {0}", $"{obj.GetType().Name} => {propertyInfo.Name}");
                }
            }
            return maxValue;
        }
    }

    public async Task<bool> ValidatePropertyReadonly(PropertyInfo propertyInfo, object obj)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(obj));

            var validators = _config.GetPropertyValidators(_logger, scope);
            bool readOnly = false;
            foreach (var validator in validators)
            {
                try
                {
                    if (validator is IUICPropertyValidationRuleReadonly readonlyValidator)
                    {
                        var result = await readonlyValidator.IsReadonly(propertyInfo, obj);
                        if (result)
                        {
                            readOnly = true;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception in validating {0}", validator.GetType().FullName);
                }

            }
            return readOnly;
        }
    }

    public async Task<bool> ValidatePropertyRequired(PropertyInfo propertyInfo, object obj)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (propertyInfo == null)
                throw new ArgumentNullException(nameof(obj));

            var validators = _config.GetPropertyValidators(_logger, scope);
            bool required = false;
            foreach (var validator in validators)
            {
                try
                {
                    if (validator is IUICPropertyValidationRuleRequired requiredValidator)
                    {
                        var result = await requiredValidator.IsRequired(propertyInfo, obj);
                        if (result)
                        {
                            required = true;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception in validating {0}", validator.GetType().FullName);
                }

            }
            return required;
        }
    }
    #endregion
}
