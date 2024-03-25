using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UIComponents.Abstractions;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Generators.Services;
using UIComponents.Generators.Services.Internal;
using UIComponents.Models.Models.Texts;

namespace UIComponents.Generators.Configuration;

public class UICConfig
{
    
    private readonly UicConfigOptions _options;
    private readonly ILogger<UICConfig> _logger;

    public UICConfig(UicConfigOptions options, IServiceProvider serviceProvider, ILogger<UICConfig> logger)
    {
        _options = options;
        ServiceProvider = serviceProvider;
        _logger = logger;
        ButtonGenerators = new(this);
    }


    public ButtonGenerator ButtonGenerators { get; }

    public IServiceProvider ServiceProvider { get; init; }
    public IUICLanguageService LanguageService
    {
        get
        {
            if (!_options.CheckLanguageServiceType)
                return new LanguageService();

            try
            {
                var languageService = ServiceProvider.GetRequiredService<IUICLanguageService>();

                if (languageService is LanguageService)
                {
                    _logger.LogError($"There is no languageservice registrated as {nameof(IUICLanguageService)}. Disable this message in the config of UIComponents => {nameof(UicConfigOptions.CheckLanguageServiceType)} = false");
                    _options.CheckLanguageServiceType = false;
                }
                    
                return languageService;

            } catch(Exception ex)
            {
                _logger.LogError(ex, $"Error getting {nameof(IUICLanguageService)}. If no service is available, disable this in the config of UIComponents => {nameof(UicConfigOptions.CheckLanguageServiceType)} = false");
                _options.CheckLanguageServiceType = false;
                return new LanguageService();
            }
            
        }
    }

    public IUICPermissionService? PermissionService
    {
        get
        {
            if (!_options.CheckPermissionServiceType)
                return null;

            try
            {
                return ServiceProvider.GetRequiredService<IUICPermissionService>();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error getting {nameof(IUICPermissionService)}. If no service is available, disable this in the config of UIComponents => {nameof(UicConfigOptions.CheckPermissionServiceType)} = false");
                _options.CheckPermissionServiceType = false;
                return null;
            }
            
        }
    }


    public bool TryGetLanguageService(out IUICLanguageService? languageService)
    {
        languageService = LanguageService;
        return languageService != null;
    }
    public bool TryGetPermissionService(out IUICPermissionService? permissionService)
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
        using var scope = ServiceProvider.CreateScope();

        var (generators, types) = _options.FindGenerators<TArgs, TResult>(args);

        foreach (var type in types)
        {
            try
            {
                var generator = (IUICGenerator<TArgs, TResult>)scope.ServiceProvider.GetRequiredService(type);

                generators.Add(generator);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to get {0} from serviceProvider", type.FullName);
            }
        }
        options.Generators.ForEach(x => {
            if (x is IUICGenerator<TArgs, TResult> generator)
                generators.Add(generator);
        });


        TConverted? result = default(TConverted?);
        if(!generators.Any())
            _logger.LogDebug("{0} does not have any matching generators", debugString);

        foreach (var generator in generators.OrderBy(x => x.Priority))
        {
            try
            {
                var generatorResult = await generator.GetResult(args, result);
                if (generatorResult.Success && generatorResult.Result != null)
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
                    if(args is UICPropertyArgs propArgs)
                    {
                        switch (propArgs.CallCollection.CurrentCallType)
                        {
                            //Do not log any debug if these calltypes have no result
                            case UICGeneratorPropertyCallType.PropertyGroupSpan:
                            case UICGeneratorPropertyCallType.PropertyTooltip:
                            case UICGeneratorPropertyCallType.PropertyInfoSpan:
                                continue;
                        }
                        var ignoreAttr = propArgs.PropertyInfo?.GetCustomAttribute<UICIgnoreAttribute>()??null;
                        if(ignoreAttr != null)
                        {
                            _logger.LogTrace("{0} is ignored by UICIgnoreAttr", propArgs.PropertyName);
                            continue;
                        }
                    }

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
        
        return await GetGeneratedResultAsync<UICPropertyArgs, IUIComponent>($"{args.CallCollection.CurrentCallType} {args.ClassObject.GetType().Name}{((args.PropertyInfo == null)?"":$" => {args.PropertyName}")}",args, options);
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

    public Task<Type?>GetForeignKeyTypeAsync(PropertyInfo propertyInfo, UICOptions options)
    {
        return GetGeneratedResultAsync<PropertyInfo, Type?>($"ForeignKeyType for {ClassAndPropertyString(propertyInfo)}", propertyInfo, options);
    }

    public Task<UICSpan?> GetPropertyGroupSpanAsync(UICPropertyArgs args, IUIComponent caller)
    {
        return GetGeneratedResultAsync<IUIComponent, UICSpan>(UICGeneratorPropertyCallType.PropertyGroupSpan, caller, args);
    }

    public Task<Translatable?> GetToolTipAsync(UICPropertyArgs args, IUIComponent caller)
    {
        return GetGeneratedResultAsync<Translatable?>(UICGeneratorPropertyCallType.PropertyTooltip, caller, args);
    }


    public Task<List<SelectListItem>?> GetSelectListItems(UICPropertyArgs args, IUIComponent caller)
    {
        return GetGeneratedResultAsync<List<SelectListItem>>(UICGeneratorPropertyCallType.SelectListItems, caller, args);
    }

    public async Task<List<SelectListItem>> GetSelectListItems(PropertyInfo propertyInfo, bool addEmptyItem)
    {
        var args = new UICPropertyArgs(null, propertyInfo, UICPropertyType.SelectList, new(), new(UICGeneratorPropertyCallType.SelectListItems, null, null), this);
        args.Options.SelectlistAddEmptyItem= addEmptyItem;

        return await GetSelectListItems(args, null)?? new();
    }

    public static string ClassAndPropertyString(PropertyInfo propertyInfo)
    {
        if (propertyInfo == null)
            return string.Empty;
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
