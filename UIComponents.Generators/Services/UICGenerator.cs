using System.Linq.Expressions;
using System.Reflection;
using UIComponents.Generators.Configuration;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;
using UIComponents.Generators.Models;
using UIComponents.Generators.Models.Arguments;
using UIComponents.Models.Models.Tables;

namespace UIComponents.Generators.Services;

public class UICGenerator : IUIComponentGenerator
{
    protected readonly UICConfig _configuration;

    public UICGenerator(UICConfig configuration)
    {
        _configuration = configuration;
    }


    #region Public methods
    public Task<IUIComponent?> CreateComponentAsync<T>(T classObject, UICOptions? options = null) where T : class
    {
        if (classObject.GetType() == typeof(string)) 
            throw new Exception($"String is not valid as {nameof(classObject)}");

        options = GetOptions(options);

        var cc = new UICCallCollection(UICGeneratorPropertyCallType.ClassObject, null, null);
        return _configuration.GetChildComponentAsync(classObject, null, options!, cc);

    }

    public Task<IUIComponent?> CreateComponentAsync<T, TProp>(T classObject, Expression<Func<T, TProp>> expression, UICOptions? options = null) where T : class
    {
        var propertyInfo = GetPropertyInfoFromExpression(expression);
        return CreateElementFromProperty(propertyInfo, classObject, options);
    }

    public Task<IUIComponent?> CreateElementFromProperty(PropertyInfo propertyInfo, object classObject, UICOptions? options = null)
    {
        options = GetOptions(options);

        var cc = new UICCallCollection(UICGeneratorPropertyCallType.PropertyGroup, null, null);
        return _configuration.GetChildComponentAsync(classObject, propertyInfo, options!, cc);
    }


    public Task<UICTableColumn> SupplementTableColumn(UICTableColumn tableColumn)
    {
        return _configuration.GenerateTableColumn(tableColumn);
    }
    #endregion


    #region Protected Methods




    protected PropertyInfo GetPropertyInfoFromExpression<T, TProp>(Expression<Func<T, TProp>> expression) where T : class
    {
        MemberExpression memberExpression = (MemberExpression)expression.Body;
        PropertyInfo propertyInfo = (PropertyInfo)memberExpression.Member;
        return propertyInfo;
    }

    

    /// <summary>
    /// If options == null, create defaultOptions
    /// </summary>
    /// <param name="options"></param>
    protected virtual UICOptions GetOptions(UICOptions? options)
    {
        if (options == null)
            options = new();
        return options;
    }

    #endregion
}
