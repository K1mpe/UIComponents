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


    #endregion

    #region Ctor
    public UICValidationService(ILogger<UICValidationService> logger, UicConfigOptions config)
    {
        _logger = logger;
        _config = config;
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
                _logger.LogError(ex, $"Exception in validating {property.DeclaringType.Name} => {property.Name}");
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
        var validators = _config.GetPropertyValidators(_logger);
        foreach (var validator in validators)
        {
            try
            {
                var propType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                if (validator.PropertyType != null && !propType.IsAssignableTo(validator.PropertyType))
                    continue;

                var validatorResult = await validator.CheckValidationErrors(propertyInfo, obj);
                foreach (var error in validatorResult.ValidationErrors)
                {
                    _logger.LogDebug($"ValidationError for {propertyInfo.DeclaringType.Name} => {propertyInfo.Name}: {error.ErrorMessage}");
                    result.AddError(error.ErrorMessage, error.Property??propertyInfo);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in validating {validator.GetType().FullName}");
            }

        }
        return result;
    }

    public async Task<int?> ValidatePropertyMaxLength(PropertyInfo propertyInfo, object obj)
    {
        if (!propertyInfo.PropertyType.IsAssignableTo(typeof(string)))
            return null;

        var validators = _config.GetPropertyValidators(_logger);
        int? maxLength = null;
        foreach(var  validator in validators)
        {
            try
            {
                if(validator is IUICPropertyValidationRuleMaxLength maxLengthValidator)
                {
                    var result = await maxLengthValidator.MaxLength(propertyInfo, obj);
                    if (result == null)
                        continue;
                    if(maxLength == null || result < maxLength)
                        maxLength = result;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Exception when validating max length of {obj.GetType().Name} => {propertyInfo.Name}");
            }
        }
        return maxLength;
    }

    public async Task<Nullable<TValueType>> ValidatePropertyMaxValue<TValueType>(PropertyInfo propertyInfo, object obj) where TValueType : struct, IComparable
    {
        var validators = _config.GetPropertyValidators(_logger);
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
                _logger.LogError(ex, $"Exception when validating max length of {obj.GetType().Name} => {propertyInfo.Name}");
            }
        }
        return maxValue;
    }

    public async Task<int?> ValidatePropertyMinLength(PropertyInfo propertyInfo, object obj)
    {
        if (!propertyInfo.PropertyType.IsAssignableTo(typeof(string)))
            return null;

        var validators = _config.GetPropertyValidators(_logger);
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
                _logger.LogError(ex, $"Exception when validating max length of {obj.GetType().Name} => {propertyInfo.Name}");
            }
        }
        return maxLength;
    }

    public async Task<Nullable<TValueType>> ValidatePropertyMinValue<TValueType>(PropertyInfo propertyInfo, object obj) where TValueType : struct, IComparable
    {
        var validators = _config.GetPropertyValidators(_logger);
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
                _logger.LogError(ex, $"Exception when validating max length of {obj.GetType().Name} => {propertyInfo.Name}");
            }
        }
        return maxValue;
    }

    public async Task<bool> ValidatePropertyReadonly(PropertyInfo propertyInfo, object obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));
        if (propertyInfo == null)
            throw new ArgumentNullException(nameof(obj));

        var validators = _config.GetPropertyValidators(_logger);
        bool readOnly = false;
        foreach (var validator in validators)
        {
            try
            {
                if(validator is IUICPropertyValidationRuleReadonly readonlyValidator)
                {
                    var result = await readonlyValidator.IsReadonly(propertyInfo, obj);
                    if(result)
                    {
                        readOnly = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception in validating {validator.GetType().FullName}");
            }

        }
        return readOnly;
    }

    public async Task<bool> ValidatePropertyRequired(PropertyInfo propertyInfo, object obj)
    {
        if (obj == null)
            throw new ArgumentNullException(nameof(obj));
        if (propertyInfo == null)
            throw new ArgumentNullException(nameof(obj));

        var validators = _config.GetPropertyValidators(_logger);
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
                _logger.LogError(ex, $"Exception in validating {validator.GetType().FullName}");
            }

        }
        return required;
    }
    #endregion
}
