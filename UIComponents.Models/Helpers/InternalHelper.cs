using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace UIComponents.Models.Helpers;

public class InternalHelper
{
    /// <summary>
    /// Copies all properties from a source to a new object. Could be used as a clone function
    /// </summary>
    /// <typeparam name="T">The output type</typeparam>
    /// <param name="source">The source object</param>
    /// <param name="includedProperties">Only copy properties with these names, If null all properties are used</param>
    /// <param name="excludedProperties">Do not copy thesse properties</param>
    /// <returns></returns>
    public static T CloneObject<T>(T target, bool deepClone, Type? type = null) where T : class
    {
        try
        {
            if (target == null)
                return null;
            T result = default(T);

            //If T is abstract (f.e. UICInput), activator cannot create a instance of abstract type
            if (type == null)
                result = (T)Activator.CreateInstance(target.GetType());
            else
                result = (T)Activator.CreateInstance(type); // If type is provided, f.e. UICInputText, a instance can be made, and cast to T (abstract type)

            return MapAllProperties(target, result, deepClone, result.GetType());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
       
    }
    public static T ConvertObject<T>(object obj) where T : class
    {
        return MapAllProperties<T>(obj, default(T), false);
    }


    public static T MapAllProperties<T>(object copyFrom, T copyTo, bool deepCopy, Type type=null) where T: class
    {
        if (copyFrom == null)
            return default(T);
        if (copyTo == null)
            copyTo = Activator.CreateInstance<T>();

        var properties = type?.GetProperties() ?? typeof(T).GetProperties();
        foreach (var property in properties)
        {
            if (!property.CanWrite)
                continue;
            var copyFromProp = copyFrom?.GetType().GetProperty(property.Name);
            if (copyFromProp == null || !copyFromProp.CanRead)
                continue;
                object value = copyFromProp.GetValue(copyFrom);
                try
                {
                    if (deepCopy && value != null && value.GetType() != typeof(string))
                    {
                        if (value is IEnumerable enumerable)
                        {

                        }
                        else if(value is Delegate)
                    {

                    }
                        else if (value.GetType().IsClass && value.GetType() != typeof(string))
                        {
                            value = CloneObject(value, true, value.GetType());
                        }
                    }
                }
                catch{

                }
            try
            {
                property.SetValue(copyTo, value, null);
            }
            catch
            {
                Console.WriteLine($"{property.DeclaringType.Name}.{property.Name} cannot write value {value?.ToString() ?? "NULL"}");
                throw;
            }
                
            
        }
        return copyTo;
    }

    public static PropertyInfo GetPropertyInfoFromExpression(Expression expression)
    {
        if(expression is UnaryExpression unaryExpression)
            return GetPropertyInfoFromExpression(unaryExpression.Operand);

        if(expression is LambdaExpression lambdaExpression)
            return GetPropertyInfoFromExpression(lambdaExpression.Body);

        if(expression is MemberExpression memberExpression)
        {
            PropertyInfo propertyInfo = (PropertyInfo)memberExpression.Member;
            return propertyInfo;
        }
        throw new Exception($"{expression.NodeType} is not yet supported");
    }
}
