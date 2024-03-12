using Microsoft.Extensions.DependencyInjection;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Models.UICGeneratorResponses;

namespace UIComponents.Generators.Configuration;

public partial class UicConfigOptions
{
    #region Fields

    private List<IUICGenerator> _generators= new();
    private List<Type> _generatorTypes= new();
    private List<Type> _objectValidators= new();
    private List<Type> _propertyValidators = new();
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

    public bool ReplaceReadMe { get; set; } = true;

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
    //public UicConfigOptions AddObjectValidator<T>() where T : IObjectValidationRule
    //{
    //    return AddValidator(typeof(T));
    //}
    //public UicConfigOptions AddPropertyValidator<T>() where T : IPropertyValidationRule
    //{
    //    return AddValidator(typeof(T));
    //}
    //public UicConfigOptions AddValidator(Type type)
    //{
    //    if(type == null)
    //        throw new ArgumentNullException(nameof(type));
    //    if (!type.IsAssignableTo(typeof(IObjectValidationRule)) && !type.IsAssignableTo(typeof(IPropertyValidationRule)))
    //        throw new ArgumentException($"{type.Name} is not assignable to {nameof(IObjectValidationRule)} or {nameof(IPropertyValidationRule)}");

    //    if(type.IsAssignableTo(typeof(IObjectValidationRule)))
    //        _objectValidators.Add(type);

    //    if (type.IsAssignableTo(typeof(IPropertyValidationRule)))
    //        _propertyValidators.Add(type);

    //    return this;
    //}
    #endregion


    #region Get methods


    public (IList<IUICGenerator<TArgs,TResult>>, IList<Type>) FindGenerators<TArgs, TResult>(TArgs args)
    {
        var filteredGenerators = _generators.Where(x => x.GetType().IsAssignableTo(typeof(IUICGenerator<TArgs, TResult>)))
            .OfType<IUICGenerator<TArgs, TResult>>();

        var filteredTypes = _generatorTypes.Where(x => x.IsAssignableTo(typeof(IUICGenerator<TArgs, TResult>)));

        return (filteredGenerators.ToList(), filteredTypes.ToList());
    }

    public IList<Type> GetPropertyValidators()
    {
        return _propertyValidators;
    }

    #endregion
}
