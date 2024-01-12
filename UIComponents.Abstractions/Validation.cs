using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace UIComponents.Abstractions;

/// <summary>
/// Validate propertyinfo 
/// </summary>
public class Validation
{

    public static bool IsRequired(PropertyInfo propertyInfo)
    {

        var v1 = propertyInfo.GetCustomAttribute<RequiredAttribute>();
        if (v1 != null)
            return true;

        var v2 = propertyInfo.GetCustomAttribute<CDCValidatorAttribute>();
        if (v2 != null)
            return v2.Required;


        if (propertyInfo.PropertyType.IsAssignableTo(typeof(Nullable<>)))
            return false;

        var foreignKey = propertyInfo.GetCustomAttribute<ForeignKeyAttribute>();
        if (foreignKey != null)
            return true;

        var fakeForeignKey = propertyInfo.GetCustomAttribute<FakeForeignKeyAttribute>();
        if (fakeForeignKey != null)
            return fakeForeignKey.IsRequired;

        if (propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType != typeof(string))
            return true;


        return false;
    }

    public static int? MinValue(PropertyInfo propertyInfo)
    {
        var cdcvalidator = propertyInfo.GetCustomAttribute<CDCValidatorAttribute>();
        if (cdcvalidator != null && cdcvalidator.MinValue != int.MinValue)
            return cdcvalidator.MinValue;

        return null;
    }
    public static int? MaxValue(PropertyInfo propertyInfo)
    {
        var cdcvalidator = propertyInfo.GetCustomAttribute<CDCValidatorAttribute>();
        if (cdcvalidator != null && cdcvalidator.MaxValue != int.MaxValue)
            return cdcvalidator.MaxValue;

        return null;
    }
}
