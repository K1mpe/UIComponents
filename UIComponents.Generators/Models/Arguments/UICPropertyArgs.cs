
using System.Reflection;
using UIComponents.Generators.Configuration;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Models.Arguments;

public class UICPropertyArgs
{
    private Type? _propertyType;
    private object _propertyValue;
    #region Ctor
    public UICPropertyArgs(object classObject, PropertyInfo? property, UICPropertyType? propertyType, UICOptions options, UICCallCollection callCollection, UICConfig configuration)
    {
        PropertyInfo = property;
        UICPropertyType= propertyType;
        ClassObject = classObject;
        Options = options;
        CallCollection = callCollection;
        Configuration = configuration;
    }
    #endregion


    #region Properties
    public PropertyInfo? PropertyInfo { get; init; }

    public object? PropertyValue { get
        {
            try
            {
                if (_propertyValue != null)
                    return _propertyValue;
                return PropertyInfo?.GetValue(ClassObject);
            }
            catch(Exception ex)
            {
                return null;
            }
        }
             }
    public Type? PropertyType => _propertyType??PropertyInfo?.PropertyType;
    public string? PropertyName => PropertyInfo?.Name;

    public UICPropertyType? UICPropertyType { get; init; }

    

    public object ClassObject { get; init; }
    public UICOptions Options { get; init; }

    public UICCallCollection CallCollection { get; init; }
    public UICConfig Configuration { get; init; }

    #endregion

    public UICPropertyArgs SetPropertyType(Type type)
    {
        _propertyType = type;
        return this;
    }
    public UICPropertyArgs SetPropertyValue(object obj)
    {
        _propertyValue = obj;
        return this;
    }
}


