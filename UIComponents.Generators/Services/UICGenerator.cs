using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using UIComponents.Abstractions.Extensions;
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
    private readonly ILogger _logger;

    public UICGenerator(UICConfig configuration, ILogger<UICGenerator> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }


    #region Public methods
    public Task<IUIComponent?> CreateComponentAsync<T>(T classObject, UICOptions? options = null) where T : class
    {
        if (classObject.GetType() == typeof(string)) 
            throw new Exception($"String is not valid as {nameof(classObject)}");

        options = GetOptions(options);

        using (_logger.BeginScopeKvp(
            new("UICObjectType", typeof(T).FullName),
            new("UICObjectName", classObject.ToString()
        )))
        {
            var cc = new UICCallCollection(UICGeneratorPropertyCallType.ClassObject, null, null);
            return _configuration.GetChildComponentAsync(classObject, null, options!, cc);
        }
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
        using (_logger.BeginScopeKvp(
            new("UICObjectType", classObject.GetType().FullName),
            new("UICObjectName", classObject.ToString()),
            new("UICPropertyName", propertyInfo.Name)
        ))
        {
            return _configuration.GetChildComponentAsync(classObject, propertyInfo, options!, cc);
        }
    }

    public async Task<Translatable> GetPropertyTranslatable(PropertyInfo propertyInfo, UICOptions? options = null)
    {
        options = GetOptions(options);

        var displayNameAttr = propertyInfo.GetCustomAttribute<DisplayNameAttribute>();
        bool hasInherit = UICInheritAttribute.TryGetInheritPropertyInfo(propertyInfo, out var inheritPropInfo);

        if (hasInherit && displayNameAttr == null)
        {
            displayNameAttr = inheritPropInfo.GetCustomAttribute<DisplayNameAttribute>();
        }

        if (displayNameAttr != null)
        {
            return displayNameAttr.DisplayName;
        }
        else
        {
            var propertyType = await _configuration.GetPropertyTypeAsync(propertyInfo, options);
            return TranslationDefaults.TranslateProperty(inheritPropInfo, propertyType);
        }
    }

    public Task<UICTableColumn> SupplementTableColumn(UICTableColumn tableColumn)
    {
        return _configuration.GenerateTableColumn(tableColumn);
    }

    /// <inheritdoc cref="CreateButtonAsync(UICGeneratorPropertyCallType, object, UICOptions?, Action{UICButton})"/>
    public Task<IUIComponent> CreateButtonAsync(UICGeneratorPropertyCallType buttonType, Action<UICButton> config) => CreateButtonAsync(buttonType, null, null, config);

    /// <summary>
    /// Use the generator the create a button
    /// </summary>
    /// <param name="buttonType">The type of button requested. Other calltypes will give a exception.</param>
    /// <param name="classobject">Optional object that can be used for specific buttontext or click actions</param>
    /// <param name="options"></param>
    /// <param name="configButton">Config only works if the result is a <see cref="UICButton"/></param>
    /// <returns></returns>
    /// <exception cref="ArgumentStringException"></exception>
    public async Task<IUIComponent> CreateButtonAsync(UICGeneratorPropertyCallType buttonType, object classobject = null, UICOptions? options = null, Action<UICButton> configButton = null)
    {
        var cc = new UICCallCollection(buttonType, null, null);
        var args = new UICPropertyArgs(classobject, null, null, GetOptions(options), cc, _configuration);
        IUIComponent result = null;
        switch (buttonType)
        {
            case UICGeneratorPropertyCallType.ButtonCancel:
                result = await _configuration.ButtonGenerators.GenerateCancelButton(args, null);
                break;
            case UICGeneratorPropertyCallType.ButtonCreate:
                result = await _configuration.ButtonGenerators.GenerateCreateButton(args, null);
                break;
            case UICGeneratorPropertyCallType.ButtonSave:
                result = await _configuration.ButtonGenerators.GenerateSaveButton(args, null);
                break;
            case UICGeneratorPropertyCallType.ButtonEditReadonly:
                result = await _configuration.ButtonGenerators.GenerateEditButton(args, null);
                break;
            case UICGeneratorPropertyCallType.ButtonDelete:
                result = await _configuration.ButtonGenerators.GenerateDeleteButton(args, null);
                break;
            default:
                throw new ArgumentStringException("{0} is not a valid calltype for a button", buttonType);
        }
        if(result is UICButton buttonResult)
        {
            configButton?.Invoke(buttonResult);
        }
        return result;
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
