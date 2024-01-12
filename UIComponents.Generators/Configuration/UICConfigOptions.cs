using Microsoft.Extensions.DependencyInjection;
using UIComponents.Abstractions.Interfaces.ExternalServices;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;
using UIComponents.Generators.Models.UICGeneratorResponses;

namespace UIComponents.Generators.Configuration;

public partial class UICConfigOptions
{
    #region Fields

    private List<IUICGenerator> _generators= new();
    private List<Type> _generatorTypes= new();
        
    
    #endregion

    #region Ctor
    public UICConfigOptions()
    {
    }
    #endregion

    public Type? LanguageServiceType;
    public Type? PermissionServiceType;
    public bool CheckLanguageServiceType;
    public bool CheckPermissionServiceType;

    public UICLanguageConfig LanguageConfig { get; set; } = new();
    public class UICLanguageConfig
    {
       
    }

    #region Add Generators
    public UICConfigOptions AddGenerator<TArgs, TResult>(string name, double priority, Func<TArgs, TResult?, Task<IUICGeneratorResponse<TResult>>> func)
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

    public UICConfigOptions AddGenerator(Type type)
    {
        InternalGeneratorHelper.CheckType<IUICGenerator>(type);
        _generatorTypes.Add(type);
        return this;
    }
    public UICConfigOptions AddGenerator<T>() where T : class, IUICGenerator
    {
        _generatorTypes.Add(typeof(T));
        return this;
    }

    public UICConfigOptions AddAndRegisterGenerator(Type type, IServiceCollection serviceCollection)
    {
        InternalGeneratorHelper.CheckType<IUICGenerator>(type);
        serviceCollection.AddScoped(type);
        AddGenerator(type);
        return this;
    }
    public UICConfigOptions AddAndRegisterGenerator<T>(IServiceCollection serviceCollection) where T : class, IUICGenerator
    {
        return AddAndRegisterGenerator(typeof(T), serviceCollection);
    }

    /// <summary>
    /// <inheritdoc cref="AddCustomGenerator{TArgs, TResult}(IUICGenerator{TArgs, TResult})"/>
    /// </summary>
    public UICConfigOptions AddGenerator<TArgs, TResult>(IUICGenerator<TArgs, TResult> generator)
    {
        return AddCustomGenerator(generator);
    }
    
    /// <summary>
    /// Add a custom Generator, Use <see cref="GeneratorHelper"/> for presets.
    /// </summary>
    public UICConfigOptions AddCustomGenerator<TArgs, TResult>(IUICGenerator<TArgs, TResult> generator)
    {
        _generators.Add(generator);

        return this;
    }

    public UICConfigOptions AddPropertyGenerator(string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> function)
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

    public UICConfigOptions AddObjectGenerator(Type type, string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> function)
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
    public UICConfigOptions AddObjectGenerator<T>(string name, double priority, Func<UICPropertyArgs, IUIComponent?, Task<IUICGeneratorResponse<IUIComponent>>> function) where T : class
    {
        return AddObjectGenerator(typeof(T), name, priority, function);
    }



    #endregion

    #region Set Services
    public UICConfigOptions SetLanguageService(Type languageServiceType)
    {
        InternalGeneratorHelper.CheckType<ILanguageService>(languageServiceType);

        LanguageServiceType= languageServiceType;
        return this;
    }
    public UICConfigOptions SetLanguageService<T>() where T: class, ILanguageService
    {
        LanguageServiceType = typeof(T);
        return this;
    }

    public UICConfigOptions SetPermissionService(Type permissionServiceType)
    {
        InternalGeneratorHelper.CheckType<IPermissionCurrentUserService>(permissionServiceType);

        PermissionServiceType = permissionServiceType;
        return this;
    }
    public UICConfigOptions SetPermissionService<T>() where T: class, IPermissionCurrentUserService
    {
        PermissionServiceType = typeof(T);
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

    #endregion
}
