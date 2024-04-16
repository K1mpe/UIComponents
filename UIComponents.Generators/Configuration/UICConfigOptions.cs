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
    /// The replace parameters only work if there is no version file in the UIComponents folder or the file is not the same version
    /// </summary>
    /// <remarks>
    /// <br><see cref="ReplaceComponents"/>   </br>
    /// <br><see cref="ReplaceCss"/>          </br>
    /// <br><see cref="ReplaceScripts"/>      </br>
    /// <br><see cref="ReplaceTaghelpers"/>   </br>
    /// <br><see cref="AddReadMe"/>           </br>
    /// <br><see cref="AddChangeLog"/>        </br>
    /// </remarks>
    public bool OnlyReplaceNewerVersion { get; set; } = true;

    /// <summary>
    /// Replace the wwwroot/uic/uic.scss
    /// </summary>
    public bool ReplaceCss { get; set; } = true;

    /// <summary>
    /// Replace the UIComponents/...
    /// </summary>
    public bool ReplaceComponents { get; set; } = true;

    /// <summary>
    /// Replace the wwwroot/uic/uic.js file
    /// </summary>
    public bool ReplaceScripts { get; set; } = true;

    public bool ReplaceTaghelpers { get; set; } = true;
    public bool AddReadMe { get; set; } = true;
    public bool AddChangeLog { get; set; }

    public bool AddFileExplorerImgs { get; set; }

    public bool CheckLanguageServiceType { get; set; } = true;
    public bool CheckPermissionServiceType { get; set; } = true;

    public bool CheckPropertyValidatorReadonly { get; set; }
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

    public UicConfigOptions AddAndRegisterValidator(Type type, IServiceCollection serviceCollection)
    {
        InternalGeneratorHelper.CheckType<IUICPropertyValidationRule>(type);
        serviceCollection.AddScoped(type);
        AddPropertyValidator(type);
        return this;
    }
    public UicConfigOptions AddAndRegisterValidator<T>(IServiceCollection serviceCollection) where T : class, IUICPropertyValidationRule
    {
        return AddAndRegisterValidator(typeof(T), serviceCollection);
    }
    /// <summary>
    /// Add a validator for a property of a object. This will be used by <see cref="IUICValidationService"/> 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public UicConfigOptions AddValidatorProperty<T>() where T : IUICPropertyValidationRule
    {
        return AddPropertyValidator(typeof(T));
    }

    /// <summary>
    /// Add a validator for a property or a object. This will be used by <see cref="IUICValidationService"/> 
    /// </summary>
    public UicConfigOptions AddPropertyValidator(Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));
        if(!type.IsAssignableTo(typeof(IUICPropertyValidationRule)))
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
        Func<PropertyInfo, object, Task<bool>> isRequiredFunc
        )
    {
        _propertyValidationRules.Add(new CustomValidatorPropertyRequired()
        {
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
        Func<PropertyInfo, object, Task<Nullable<TValue>>> minValueFunc
        ) where TValue : struct, IComparable
    {
        _propertyValidationRules.Add(new CustomValidatorPropertyMinValue<TValue>()
        {
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
        Func<PropertyInfo, object, Task<Nullable<TValue>>> maxValueFunc
        ) where TValue : struct, IComparable
    {
        _propertyValidationRules.Add(new CustomValidatorPropertyMaxValue<TValue>()
        {
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
        Func<PropertyInfo, object, Task<int?>> minLengthFunc
        )
    {
        _propertyValidationRules.Add(new CustomValidatorPropertyMinLength()
        {
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
        Func<PropertyInfo, object, Task<int?>> maxLengthFunc
        )
    {
        _propertyValidationRules.Add(new CustomValidatorPropertyMaxLength()
        {
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
        Func<PropertyInfo, object, Task<bool>> readonlyFunc
        )
    {
        _propertyValidationRules.Add(new CustomValidatorPropertyReadonly()
        {
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


    public IList<IUICPropertyValidationRule> GetPropertyValidators(ILogger logger, IServiceScope scope)
    {
        List<IUICPropertyValidationRule> rules = new List<IUICPropertyValidationRule>();
        rules.AddRange(_propertyValidationRules);
        foreach(var type in _propertyValidators)
        {
            try
            {
                var instance = (IUICPropertyValidationRule)scope.ServiceProvider.GetService(type);
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
