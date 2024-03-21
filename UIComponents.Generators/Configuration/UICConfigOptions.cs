using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Models.UICGeneratorResponses;
using UIComponents.Generators.Validators.CustomValidators;

namespace UIComponents.Generators.Configuration;

public partial class UicConfigOptions
{
    #region Fields

    private List<IUICGenerator> _generators= new();
    private List<Type> _generatorTypes= new();
    private List<Type> _propertyValidators = new();
    private List<IUICPropertyValidationRule> _propertyValidationRules= new();
    #endregion

    #region Ctor
    public UicConfigOptions()
    {
    }
    #endregion


   

    #region Properties
    /// <summary>
    /// Replace the wwwroot/uic/uic.js file
    /// </summary>
    public bool ReplaceScripts { get; set; } = true;

    /// <summary>
    /// Replace the wwwroot/uic/uic.scss
    /// </summary>
    public bool ReplaceCss { get; set; } = true;

    /// <summary>
    /// Replace the UIComponents/...
    /// </summary>
    public bool ReplaceComponents { get; set; } = true;

    public bool AddReadMe { get; set; }
    public bool AddChangeLog { get; set; }

    public bool CheckLanguageServiceType { get; set; } = true;
    public bool CheckPermissionServiceType { get; set; } = true;
    #endregion

    #region Add Generators
    public UicConfigOptions AddGenerator<TArgs, TResult>(string name, double priority, Func<TArgs, TResult?, Task<IUICGeneratorResponse<TResult>>> func)
    {
        var generator = new UICCustomGenerator<TArgs, TResult>()
        {
            Priority = priority,
            Name = name,
            GetResult = func
        };
        _generators.Add(generator);
        return this;
    }

    public UicConfigOptions AddGenerator(Type type)
    {
        InternalGeneratorHelper.CheckType<IUICGenerator>(type);
        _generatorTypes.Add(type);
        return this;
    }
    public UicConfigOptions AddGenerator<T>() where T : class, IUICGenerator
    {
        _generatorTypes.Add(typeof(T));
        return this;
    }

    public UicConfigOptions AddAndRegisterGenerator(Type type, IServiceCollection serviceCollection)
    {
        InternalGeneratorHelper.CheckType<IUICGenerator>(type);
        serviceCollection.AddScoped(type);
        AddGenerator(type);
        return this;
    }
    public UicConfigOptions AddAndRegisterGenerator<T>(IServiceCollection serviceCollection) where T : class, IUICGenerator
    {
        return AddAndRegisterGenerator(typeof(T), serviceCollection);
    }

    /// <summary>
    /// <inheritdoc cref="AddCustomGenerator{TArgs, TResult}(IUICGenerator{TArgs, TResult})"/>
    /// </summary>
    public UicConfigOptions AddGenerator<TArgs, TResult>(IUICGenerator<TArgs, TResult> generator)
    {
        return AddCustomGenerator(generator);
    }
    
    /// <summary>
    /// Add a custom Generator, Use <see cref="GeneratorHelper"/> for presets.
    /// </summary>
    public UicConfigOptions AddCustomGenerator<TArgs, TResult>(IUICGenerator<TArgs, TResult> generator)
    {
        _generators.Add(generator);

        return this;
    }

    public UicConfigOptions AddPropertyGenerator(string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> function)
    {
        var generator = new UICCustomGenerator<UICPropertyArgs, IUIComponent>()
        {
            Name = name,
            Priority = priority,
            GetResult = function
        };

        _generators.Add(generator);

        return this;
    }

    public UicConfigOptions AddObjectGenerator(Type type, string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> function)
    {
        var generator = new UICCustomGenerator<UICPropertyArgs, IUIComponent>()
        {
            Name = name,
            Priority = priority,
            GetResult = async (args, existing) =>
            {
                if(!args.ClassObject.GetType().IsAssignableTo(type) && args.PropertyInfo != null)
                    return new UICGeneratorResponseNext<IUIComponent>();

                if (!args.PropertyType?.IsAssignableTo(type)??false)
                    return new UICGeneratorResponseNext<IUIComponent>();

                return await function(args, existing);
            }
        };
        _generators.Add(generator);
        return this;
    }
    public UicConfigOptions AddObjectGenerator<T>(string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> function) where T : class
    {
        return AddObjectGenerator(typeof(T), name, priority, function);
    }



    #endregion

    #region Add Validators

    /// <summary>
    /// Add a validator for a property of a object. This will be used by <see cref="IUICValidationService"/> 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public UicConfigOptions AddValidatorProperty<T>() where T : IUICPropertyValidationRule
    {
        return AddValidator(typeof(T));
    }

    /// <summary>
    /// Add a validator for a property or a object. This will be used by <see cref="IUICValidationService"/> 
    /// </summary>
    public UicConfigOptions AddValidator(Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));
            throw new ArgumentException($"{type.Name} is not assignable to {nameof(IUICPropertyValidationRule)}");

        if (type.IsAssignableTo(typeof(IUICPropertyValidationRule)))
            _propertyValidators.Add(type);

        return this;
    }

    /// <summary>
    /// Used by <see cref="IUICValidationService.ValidateObjectProperty(PropertyInfo, object)"/> and <see cref="IUICValidationService.ValidatePropertyRequired(PropertyInfo)"/>
    /// </summary>
    /// <param name="checkValidationErrorsFunc">Function that validates property of object</param>
    /// <param name="isRequiredFunc">Used by <see cref="UICInput"/> to set required property</param>
    /// <returns></returns>
    public UicConfigOptions AddValidatorPropertyRequired(
        Func<PropertyInfo, object, Task<bool>> isRequiredFunc,
        Func<PropertyInfo, object, Task<ValidationRuleResult>> checkValidationErrorsFunc
        )
    {
        _propertyValidationRules.Add(new CustomValidatorPropertyRequired()
        {
            CheckValidationErrorsFunc = checkValidationErrorsFunc,
            IsRequiredFunc = isRequiredFunc
        });
        return this;
    }

    /// <summary>
    /// Used by <see cref="IUICValidationService.ValidateObjectProperty(PropertyInfo, object)"/> and <see cref="IUICValidationService.ValidatePropertyMinValue{TValueType}(PropertyInfo)"/>
    /// </summary>
    /// <param name="checkValidationErrorsFunc">Function that validates property of object</param>
    /// <param name="minValueFunc">Used by <see cref="UICInput"/> to set MinValue property</param>
    /// <returns></returns>
    public UicConfigOptions AddValidatorPropertyMinValue<TValue>(
        Func<PropertyInfo, object, Task<Nullable<TValue>>> minValueFunc,
        Func<PropertyInfo, object, Task<ValidationRuleResult>> checkValidationErrorsFunc = null
        ) where TValue : struct, IComparable
    {
        _propertyValidationRules.Add(new CustomValidatorPropertyMinValue<TValue>()
        {
            CheckValidationErrorsFunc = checkValidationErrorsFunc,
            MinValueFunc = minValueFunc
        });
        return this;
    }

    /// <summary>
    /// Used by <see cref="IUICValidationService.ValidateObjectProperty(PropertyInfo, object)"/> and <see cref="IUICValidationService.ValidatePropertyMaxValue{TValueType}(PropertyInfo)"/>
    /// </summary>
    /// <param name="checkValidationErrorsFunc">Function that validates property of object</param>
    /// <param name="maxValueFunc">Used by <see cref="UICInput"/> to set MaxValue property</param>
    /// <returns></returns>
    public UicConfigOptions AddValidatorPropertyMaxValue<TValue>(
        Func<PropertyInfo, object, Task<Nullable<TValue>>> maxValueFunc,
        Func<PropertyInfo, object, Task<ValidationRuleResult>> checkValidationErrorsFunc = null
        ) where TValue : struct, IComparable
    {
        _propertyValidationRules.Add(new CustomValidatorPropertyMaxValue<TValue>()
        {
            CheckValidationErrorsFunc = checkValidationErrorsFunc,
            MaxValueFunc = maxValueFunc
        });
        return this;
    }

    /// <summary>
    /// Used by <see cref="IUICValidationService.ValidateObjectProperty(PropertyInfo, object)"/> and <see cref="IUICValidationService.ValidatePropertyMinLength(PropertyInfo)"/>
    /// </summary>
    /// <param name="checkValidationErrorsFunc">Function that validates property of object</param>
    /// <param name="minLengthFunc">Used by <see cref="UICInputText.ValidationMinLength"/></param>
    /// <returns></returns>
    public UicConfigOptions AddValidatorPropertyMinLength(
        Func<PropertyInfo, object, Task<int?>> minLengthFunc,
        Func<PropertyInfo, object, Task<ValidationRuleResult>> checkValidationErrorsFunc = null
        )
    {
        _propertyValidationRules.Add(new CustomValidatorPropertyMinLength()
        {
            CheckValidationErrorsFunc = checkValidationErrorsFunc,
            MinLengthFunc = minLengthFunc
        });
        return this;
    }

    /// <summary>
    /// Used by <see cref="IUICValidationService.ValidateObjectProperty(PropertyInfo, object)"/> and <see cref="IUICValidationService.ValidatePropertyMaxLength(PropertyInfo)"/>
    /// </summary>
    /// <param name="checkValidationErrorsFunc">Function that validates property of object</param>
    /// <param name="maxLengthFunc">Used by <see cref="UICInputText.ValidationMinLength"/></param>
    /// <returns></returns>
    public UicConfigOptions AddValidatorPropertyMaxLength(
        Func<PropertyInfo, object, Task<int?>> maxLengthFunc,
        Func<PropertyInfo, object, Task<ValidationRuleResult>> checkValidationErrorsFunc = null
        )
    {
        _propertyValidationRules.Add(new CustomValidatorPropertyMaxLength()
        {
            CheckValidationErrorsFunc = checkValidationErrorsFunc,
            MaxLengthFunc = maxLengthFunc
        });
        return this;
    }


    /// <summary>
    /// Used by <see cref="IUICValidationService.ValidateObjectProperty(PropertyInfo, object)"/> and <see cref="IUICValidationService.ValidatePropertyReadonly(PropertyInfo)"/>
    /// </summary>
    /// <param name="checkValidationErrorsFunc">Function that validates property of object</param>
    /// <param name="readonlyFunc">Used by <see cref="UICInput.Readonly"/></param>
    /// <remarks>
    /// <see cref="IUICPermissionService.CanEditObject{T}(T)"/> and <see cref="IUICPermissionService.CanEditProperty{T}(T, string)"/> is already checked
    /// </remarks>
    /// <returns></returns>
    public UicConfigOptions AddValidatorPropertyReadonly(
        Func<PropertyInfo, object, Task<bool>> readonlyFunc,
        Func<PropertyInfo, object, Task<ValidationRuleResult>> checkValidationErrorsFunc
        )
    {
        _propertyValidationRules.Add(new CustomValidatorPropertyReadonly()
        {
            CheckValidationErrorsFunc = checkValidationErrorsFunc,
            IsReadonlyFunc = readonlyFunc
        });
        return this;
    }

    #endregion


    #region Get methods


    public (IList<IUICGenerator<TArgs,TResult>>, IList<Type>) FindGenerators<TArgs, TResult>(TArgs args)
    {
        var filteredGenerators = _generators.Where(x => x.GetType().IsAssignableTo(typeof(IUICGenerator<TArgs, TResult>)))
            .OfType<IUICGenerator<TArgs, TResult>>();

        var filteredTypes = _generatorTypes.Where(x => x.IsAssignableTo(typeof(IUICGenerator<TArgs, TResult>)));

        return (filteredGenerators.ToList(), filteredTypes.ToList());
    }


    public IList<IUICPropertyValidationRule> GetPropertyValidators(ILogger logger)
    {
        List<IUICPropertyValidationRule> rules = new List<IUICPropertyValidationRule>();
        rules.AddRange(_propertyValidationRules);
        foreach(var type in _propertyValidators)
        {
            try
            {
                var instance = (IUICPropertyValidationRule)Activator.CreateInstance(type);
                rules.Add(instance);
            }
            catch(Exception ex)
            {
                logger.LogError(ex, $"Failed to create instance of {type.FullName}");
            }
        }
        return rules;

    }

    #endregion
}
