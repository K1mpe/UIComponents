using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace UIComponents.Abstractions.Attributes;

/// <summary>
/// Inherit the attributes of another property.
/// <br>This can be placed on a property or a class.</br>
/// </summary>
/// <remarks>
/// Supported features:
/// <br><see cref="Translatable"/>from other property</br>
/// <br>Validation attributes</br>
/// <br>Permission requests</br>
/// <br><see cref="UICPropertyTypeAttribute"/></br>
/// <br><see cref="UICSpanAttribute"/></br>
/// <br><see cref="UICTooltipAttribute"/></br>
/// <br><see cref="ForeignKeyAttribute"/></br>
/// <br><see cref="FakeForeignKeyAttribute"/></br>
/// </remarks>
public class UICInheritAttribute : Attribute
{
    #region Ctor
    public UICInheritAttribute(Type type, string propertyName)
    {
        Types = new[] { type };
        PropertyName = propertyName;
    }

    public UICInheritAttribute(params Type[] types)
    {
        Types = types;
    }
    #endregion



    #region Properties

    public Type[] Types { get; set; }

    public string PropertyName { get; set; }

    #endregion
    /// <summary>
    /// Try to find the InheritProperty. First Try on the property, Then try on the class
    /// </summary>
    /// <remarks>
    /// When failed, the out property 'inherit' is the current property info. So inherit is never null
    /// </remarks>
    public static bool TryGetInheritPropertyInfo(PropertyInfo propertyInfo, out PropertyInfo inherit)
    {
        if (propertyInfo == null)
            throw new ArgumentNullException(nameof(propertyInfo));

        var attr = propertyInfo.GetCustomAttribute<UICInheritAttribute>();
        if(attr != null)
        {
            var property = attr.Types[0].GetProperty(attr.PropertyName);
            if(property != null && property.PropertyType.IsAssignableTo(propertyInfo.PropertyType))
            {
                inherit = property;
                return true;
            }
        }
        
        var classAttr = propertyInfo.DeclaringType.GetCustomAttribute<UICInheritAttribute>();
        if(classAttr != null)
        {
            foreach(var type in classAttr.Types)
            {
                var property2 = propertyInfo.DeclaringType.GetProperty(propertyInfo.Name);
                if(property2 != null && property2.PropertyType.IsAssignableTo(propertyInfo.PropertyType))
                {
                    inherit = property2;
                    return true;
                }
            }
        }

        inherit = propertyInfo;
        return false;
    }
}
