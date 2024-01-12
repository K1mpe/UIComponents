using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.ExternalServices;
using UIComponents.Models.Models.Texts;

namespace UIComponents.Generators.Configuration;

public class UICConfig
{
    private readonly IServiceProvider _serviceProvider;
    private readonly UICConfigOptions _options;
    private readonly ILogger<UICConfig> _logger;

    public UICConfig(UICConfigOptions options, IServiceProvider serviceProvider, ILogger<UICConfig> logger)
    {
        _options = options;
        _serviceProvider = serviceProvider;
        _logger = logger;
        ButtonGenerators = new(this);
    }


    public ButtonGenerator ButtonGenerators { get; }

    public ILanguageService? LanguageService
    {
        get
        {
            if (!_options.CheckLanguageServiceType)
                return null;

            if (_options.LanguageServiceType == null)
            {
                _logger.LogError($"There is no {nameof(ILanguageService)} provided in {nameof(UICConfigOptions)}. Assign a {nameof(ILanguageService)} or set {nameof(UICConfigOptions)}.{nameof(UICConfigOptions.CheckLanguageServiceType)} to false");
                _options.CheckLanguageServiceType = false;
                return null;
            }

            return (ILanguageService?)_serviceProvider.GetService(_options.LanguageServiceType);
        }
    }

    public IPermissionCurrentUserService? PermissionService
    {
        get
        {
            if (!_options.CheckPermissionServiceType)
                return null;

            if (_options.PermissionServiceType == null)
            {
                _logger.LogError($"There is no permissionservice provided in {nameof(UICConfigOptions)}. Assign a permissionservice or set {nameof(UICConfigOptions)}.{nameof(UICConfigOptions.CheckPermissionServiceType)} to false");
                _options.CheckPermissionServiceType = false;
                return null;
            }
            
            return (IPermissionCurrentUserService?)_serviceProvider.GetService(_options.PermissionServiceType);
        }
    }


    public bool TryGetLanguageService(out ILanguageService? languageService)
    {
        languageService = LanguageService;
        return languageService != null;
    }
    public bool TryGetPermissionService(out IPermissionCurrentUserService? permissionService)
    {
        permissionService = PermissionService;
        return permissionService != null;
    }

    public Task<TResult?> GetGeneratedResultAsync<TArgs, TResult>(string debugString, TArgs args, UICOptions options)
    {
        return GetGeneratedResultAsync<TArgs, TResult, TResult>(debugString, args, options);
    }
    public async Task<TConverted?> GetGeneratedResultAsync<TArgs, TResult, TConverted>(string debugString, TArgs args, UICOptions options) where TConverted : TResult
    {
        using var scope = _serviceProvider.CreateScope();

        var (generators, types) = _options.FindGenerators<TArgs, TResult>(args);

        foreach (var type in types)
        {
            var generator = (IUICGenerator<TArgs, TResult>)scope.ServiceProvider.GetRequiredService(type);

            generators.Add(generator);
        }
        options.Generators.ForEach(x => {
            if (x is IUICGenerator<TArgs, TResult> generator)
                generators.Add(generator);
        });


        TConverted? result = default(TConverted?);
        if(!generators.Any())
            _logger.LogError("{0} does not have any matching generators", debugString);

        foreach (var generator in generators.OrderBy(x => x.Priority))
        {
            try
            {
                var generatorResult = await generator.GetResult(args, result);
                if (generatorResult.Success)
                {
                    _logger.LogTrace("{0} Successfull result from generator {1} {2}: {3}",debugString, generator.Priority, generator.Name, generatorResult.Result?.ToString() ?? "null");
                    result = (TConverted?)generatorResult.Result;
                }


                if (!generatorResult.Continue)
                {
                    _logger.LogTrace("{0} Generating stopped after {1} {2}", debugString, generator.Priority, generator.Name);
                    break;
                }

                if (result == null)
                {
                    if (!generators.Where(x => x.Priority > generator.Priority).Any())
                        _logger.LogDebug("{0} Generators did not find a result", debugString);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{0} Error in generator {1} ", debugString, generator.Name);
            }

        }
        return result;
    }

    public async Task<IUIComponent?> GetChildComponentAsync(object parentObject, PropertyInfo? propertyInfo, UICOptions options, UICCallCollection cc)
    {
        UICPropertyType? propType = null;
        if (propertyInfo != null)
            propType = await GetPropertyTypeAsync(propertyInfo, options);

        var args = new UICPropertyArgs(parentObject, propertyInfo, propType, options, cc, this);
        
        return await GetGeneratedResultAsync<UICPropertyArgs, IUIComponent>($"{args.CallCollection.CurrentCallType} {args.ClassObject.GetType().Name}{((args.PropertyInfo == null)?"":$" => {args.PropertyInfo.Name}")}",args, options);
    }

    public Task<TResult?> GetGeneratedResultAsync<TResult>(UICGeneratorPropertyCallType callType, IUIComponent caller, UICPropertyArgs oldArgs)
    {
        return GetGeneratedResultAsync<TResult, TResult>(callType, caller, oldArgs);
    }
    public Task<TConverted?> GetGeneratedResultAsync<TResult, TConverted>(UICGeneratorPropertyCallType callType, IUIComponent caller, UICPropertyArgs oldArgs) where TConverted : TResult
    {
        var cc = new UICCallCollection(callType, caller, oldArgs.CallCollection);
        var args = new UICPropertyArgs(oldArgs.ClassObject, oldArgs.PropertyInfo, oldArgs.UICPropertyType, oldArgs.Options, cc, this);
        return GetGeneratedResultAsync<UICPropertyArgs, TResult, TConverted>($"{callType} {ClassAndPropertyString(oldArgs.PropertyInfo)}", args, args.Options);
    }

    public Task<UICPropertyType?> GetPropertyTypeAsync(PropertyInfo propertyInfo, UICOptions options)
    {
        return GetGeneratedResultAsync<PropertyInfo, UICPropertyType?>($"UICPropertyType for {ClassAndPropertyString(propertyInfo)}", propertyInfo, options);
    }

    public Task<UICSpan?> GetPropertyGroupSpanAsync(UICPropertyArgs args, IUIComponent caller)
    {
        return GetGeneratedResultAsync<IUIComponent, UICSpan>(UICGeneratorPropertyCallType.PropertyGroupSpan, caller, args);
    }

    public Task<ITranslationModel?> GetToolTipAsync(UICPropertyArgs args, IUIComponent caller)
    {
        return GetGeneratedResultAsync<ITranslationModel?>(UICGeneratorPropertyCallType.PropertyTooltip, caller, args);
    }

    /// <summary>
    /// Check if a property is required, if no result is found return false
    /// </summary>
    /// <param name="oldArgs"></param>
    /// <param name="caller"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public Task<bool?> IsPropertyRequired(UICPropertyArgs oldArgs,  IUIComponent caller)
    {
        return GetGeneratedResultAsync<bool?>(UICGeneratorPropertyCallType.PropertyRequired, caller, oldArgs);
    }

    public Task<List<SelectListItem>?> GetSelectListItems(UICPropertyArgs args, IUIComponent caller)
    {
        return GetGeneratedResultAsync<List<SelectListItem>>(UICGeneratorPropertyCallType.SelectListItems, caller, args);
    }

    public static string ClassAndPropertyString(PropertyInfo propertyInfo)
    {
        return $"{propertyInfo.DeclaringType?.Name} => {propertyInfo.Name}";
    }


    public class ButtonGenerator
    {
        private readonly UICConfig _config;

        public ButtonGenerator(UICConfig config)
        {
            _config = config;
        }

        public Task<IUIComponent?> GenerateCreateButton(UICPropertyArgs args, IUIComponent caller)
        {
            return _config.GetGeneratedResultAsync<IUIComponent>(UICGeneratorPropertyCallType.ButtonCreate, caller, args);
        }
        public Task<IUIComponent?> GenerateSaveButton(UICPropertyArgs args, IUIComponent caller)
        {
            return _config.GetGeneratedResultAsync<IUIComponent>(UICGeneratorPropertyCallType.ButtonSave, caller, args);
        }
        public Task<IUIComponent?> GenerateDeleteButton(UICPropertyArgs args, IUIComponent caller)
        {
            return _config.GetGeneratedResultAsync<IUIComponent>(UICGeneratorPropertyCallType.ButtonDelete, caller, args);
        }
        public Task<IUIComponent?> GenerateEditButton(UICPropertyArgs args, IUIComponent caller)
        {
            return _config.GetGeneratedResultAsync<IUIComponent>(UICGeneratorPropertyCallType.ButtonEditReadonly, caller, args);
        }
        public Task<IUIComponent?> GenerateCancelButton(UICPropertyArgs args, IUIComponent caller)
        {
            return _config.GetGeneratedResultAsync<IUIComponent>(UICGeneratorPropertyCallType.ButtonCancel, caller, args);
        }

    }
}
