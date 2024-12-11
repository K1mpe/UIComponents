using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Validators;

public class UICValidatorRequired : IUICPropertyValidationRuleRequired
{
    private readonly ILogger _logger;
    private readonly IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleRequired> _defaultValidator;
    public UICValidatorRequired(IUICDefaultCheckValidationErrors<IUICPropertyValidationRuleRequired> defaultValidator, ILogger<UICValidatorRequired> logger)
    {
        _defaultValidator = defaultValidator;
        _logger = logger;
    }

    public Type? PropertyType => typeof(object);

    public async Task<bool> IsRequired(PropertyInfo propertyInfo, object obj)
    {
        await Task.Delay(0);
        var v1 = propertyInfo.GetCustomAttribute<RequiredAttribute>();
        if (v1 != null)
        {
            _logger.LogDebug($"{{0}} is required => has {nameof(RequiredAttribute)}", $"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name}");
            return true;
        }
            


        if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null) //nullable type
        {
            _logger.LogDebug("{0} is NOT required => has nullable type", $"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name}");
            return false;
        }
            

        var foreignKey = propertyInfo.GetCustomAttribute<ForeignKeyAttribute>();
        if (foreignKey != null)
        {
            _logger.LogDebug($"{{0}} is required => has {nameof(ForeignKeyAttribute)}", $"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name}");
            return true;
        }
            

        var fakeForeignKey = propertyInfo.GetCustomAttribute<FakeForeignKeyAttribute>();
        if (fakeForeignKey != null)
        {
            if(fakeForeignKey.IsRequired)
                _logger.LogDebug($"{{0}} is required => has {nameof(FakeForeignKeyAttribute)}", $"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name}");
            return fakeForeignKey.IsRequired;
        }
            


        if (UICInheritAttribute.TryGetInheritPropertyInfo(propertyInfo, out var inherit))
        {
            var v2 = inherit.GetCustomAttribute<RequiredAttribute>();
            if (v2 != null)
            {
                _logger.LogDebug($"{{0}} is required because of {nameof(UICInheritAttribute)} => has {nameof(RequiredAttribute)}", $"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name}");
                return true;
            }
               


            if (inherit.PropertyType.IsAssignableTo(typeof(Nullable<>)))
            {
                _logger.LogDebug($"{{0}} is NOT required because of {nameof(UICInheritAttribute)} => has nullable type", $"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name}");
                return false;
            }
                

            var foreignKey2 = inherit.GetCustomAttribute<ForeignKeyAttribute>();
            if (foreignKey2 != null)
            {
                _logger.LogDebug($"{{0}} is required because of {nameof(UICInheritAttribute)} => has {nameof(ForeignKeyAttribute)}", $"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name}");
                return true;
            }
                

            var fakeForeignKey2 = inherit.GetCustomAttribute<FakeForeignKeyAttribute>();
            if (fakeForeignKey2 != null)
            {
                if(fakeForeignKey2.IsRequired)
                    _logger.LogDebug($"{{0}} is required because of {nameof(UICInheritAttribute)} => has {nameof(FakeForeignKeyAttribute)}", $"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name}");
                return fakeForeignKey2.IsRequired;
            }
        }
        if(propertyInfo.PropertyType.IsClass)
        {
            _logger.LogDebug($"{{0}} is required because it is a class and not nullable", $"{propertyInfo.DeclaringType.Name}.{propertyInfo.Name}");
            return true;
        }

        return false;
    }
}
