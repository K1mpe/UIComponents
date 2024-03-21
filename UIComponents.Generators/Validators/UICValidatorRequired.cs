using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Validators;

public class UICValidatorRequired : IUICPropertyValidationRuleRequired
{
    public Type? PropertyType => typeof(object);

    public Task<ValidationRuleResult> CheckValidationErrors(PropertyInfo propertyInfo, object obj)
    {
        return IUICPropertyValidationRuleRequired.DefaultValidationErrors(this, propertyInfo, obj);
    }

    public async Task<bool> IsRequired(PropertyInfo propertyInfo, object obj)
    {
        await Task.Delay(0);
        var v1 = propertyInfo.GetCustomAttribute<RequiredAttribute>();
        if (v1 != null)
            return true;


        if (propertyInfo.PropertyType.IsAssignableTo(typeof(Nullable<>)))
            return false;

        var foreignKey = propertyInfo.GetCustomAttribute<ForeignKeyAttribute>();
        if (foreignKey != null)
            return true;

        var fakeForeignKey = propertyInfo.GetCustomAttribute<FakeForeignKeyAttribute>();
        if (fakeForeignKey != null)
            return fakeForeignKey.IsRequired;


        if (UICInheritAttribute.TryGetInheritPropertyInfo(propertyInfo, out var inherit))
        {
            var v2 = inherit.GetCustomAttribute<RequiredAttribute>();
            if (v2 != null)
                return true;


            if (inherit.PropertyType.IsAssignableTo(typeof(Nullable<>)))
                return false;

            var foreignKey2 = inherit.GetCustomAttribute<ForeignKeyAttribute>();
            if (foreignKey2 != null)
                return true;

            var fakeForeignKey2 = inherit.GetCustomAttribute<FakeForeignKeyAttribute>();
            if (fakeForeignKey2 != null)
                return fakeForeignKey2.IsRequired;

        }
        return false;
    }
}
