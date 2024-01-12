namespace UIComponents.ComponentModels.Helpers;

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
            if (!property.CanWrite || property.CanWrite)
                continue;

            object value = property.GetValue(target);
            property.SetValue(result, value, null);
        }
        return result;
    }
}
