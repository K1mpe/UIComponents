using System.Linq.Expressions;
using System.Reflection;
using UIComponents.Generators.Models;

namespace UIComponents.Generators.Interfaces;

public interface IUIComponentService
{

    /// <summary>
    /// Create a component from a given object
    /// </summary>
    /// <typeparam name="T">Type of the given object</typeparam>
    /// <param name="classObject">The object used to create the component</param>
    /// <param name="options">Options with parameters to change the behavior of this service</param>
    /// <returns></returns>
    Task<IUIComponent?> CreateComponentAsync<T>(T classObject, UICOptions? options = null) where T : class;

    /// <summary>
    /// Create a component from a property of a given object
    /// </summary>
    /// <typeparam name="T">Type of the object</typeparam>
    /// <typeparam name="TProp">Type of the property</typeparam>
    /// <param name="classObject">The object that contains the property</param>
    /// <param name="expression">A expression directing to the property that should be rendered</param>
    /// <param name="options">Options with parameters to change the behavior of this service</param>
    /// <returns></returns>
    Task<IUIComponent?> CreateComponentAsync<T, TProp>(T classObject, Expression<Func<T, TProp>> expression, UICOptions? options = null) where T : class;


    /// <summary>
    /// Create a component from a property of a given object
    /// </summary>
    /// <param name="propertyInfo">The propertyinfo of the property that you want to use</param>
    /// <param name="classObject">The object contains the propertyInfo</param>
    /// <param name="options">Options with parameters to change the behavior of this service</param>
    /// <returns></returns>
    Task<IUIComponent?> CreateElementFromProperty(PropertyInfo propertyInfo, object classObject, UICOptions? options = null);
}
