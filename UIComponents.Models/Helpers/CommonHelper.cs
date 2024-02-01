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
    public static T CopyObject<T>(T target)
    {
        T result = Activator.CreateInstance<T>();
        foreach (var property in typeof(T).GetProperties())
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
