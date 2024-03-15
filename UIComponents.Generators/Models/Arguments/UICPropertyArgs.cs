
using System.Reflection;
using UIComponents.Generators.Configuration;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Models.Arguments;

public class UICPropertyArgs
{
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
                return PropertyInfo?.GetValue(ClassObject);
            }
            catch(Exception ex)
            {
                return null;
            }
        }
             }
    public Type? PropertyType => PropertyInfo?.PropertyType;
    public string? PropertyName => PropertyInfo?.Name;

    public UICPropertyType? UICPropertyType { get; init; }

    

    public object ClassObject { get; init; }
    public UICOptions Options { get; init; }

    public UICCallCollection CallCollection { get; init; }
    public UICConfig Configuration { get; init; }

    #endregion
}


