using System.Reflection;

namespace UIComponents.Abstractions.Extensions;

public static class PropertyExpressions
{
    /// <summary>
    /// Get the attribute from this propertyinfo. If no attribute is found, this can also return a attribute on the reference from <see cref="UICInheritAttribute"/>
    /// </summary>
    public static T? GetInheritAttribute<T>(this PropertyInfo propertyInfo) where T : Attribute
    {
        var attr = propertyInfo.GetCustomAttribute<T>();
        if(attr != null)
            return attr;

        if(UICInheritAttribute.TryGetInheritPropertyInfo(propertyInfo, out var inherit))
        {
            attr = inherit.GetCustomAttribute<T>();
            return attr;
        }
        return null;
    }

    /// <summary>
    /// Get the attribute from this propertyinfo. If no attribute is found, this can also return a attribute on the reference from <see cref="UICInheritAttribute"/>
    /// </summary>
    public static IEnumerable<T> GetInheritAttributes<T>(this PropertyInfo propertyInfo) where T: Attribute
    {
        var attr = propertyInfo.GetCustomAttributes<T>().ToList();

        var prop2 = propertyInfo;
        while(UICInheritAttribute.TryGetInheritPropertyInfo(prop2, out var inherit))
        {
            attr.AddRange(inherit.GetCustomAttributes<T>());
            prop2 = inherit;
        }
        return attr;
    }

}
