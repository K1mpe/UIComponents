using System.Text.Json;

namespace UIComponents.Models.Helpers;

public class CommonHelper
{
    /// <summary>
    /// Copies all properties from a source to a new object. Could be used as a clone function
    /// </summary>
    /// <typeparam name="T">The output type</typeparam>
    /// <param name="source">The source object</param>
    /// <param name="includedProperties">Only copy properties with these names, If null all properties are used</param>
    /// <param name="excludedProperties">Do not copy thesse properties</param>
    /// <returns></returns>
    public static T CopyObject<T>(T target, Type? type = null)
    {

        T result = default;

        //If T is abstract (f.e. UICInput), activator cannot create a instance of abstract type
        if (type == null)
            Activator.CreateInstance<T>();
        else 
            result = (T)Activator.CreateInstance(type); // If type is provided, f.e. UICInputText, a instance can be made, and cast to T (abstract type)

        var properties = type.GetProperties() ?? typeof(T).GetProperties();
        foreach (var property in properties)
        {
            if (!property.CanWrite || !property.CanRead)
                continue;

            object value = property.GetValue(target);
            property.SetValue(result, value, null);
        }
        return result;
    }


    public static T Convert<T>(object source)
    {
        if (source == null)
            return default;

        T result = Activator.CreateInstance<T>();
        foreach (var property in typeof(T).GetProperties())
        {
            var sourceProp = source.GetType().GetProperty(property.Name);
            if (sourceProp == null)
                continue;
            if (!property.CanWrite || !sourceProp.CanRead)
                continue;

            object value = sourceProp.GetValue(source);
            property.SetValue(result, value, null);
            if (property.PropertyType.IsAssignableTo(typeof(Dictionary<string, string>)))
            {
                var serialized = JsonSerializer.Serialize(value);
                property.SetValue(result, JsonSerializer.Deserialize(serialized, property.PropertyType));
            }

            
        }
        return result;
        
    }
}
