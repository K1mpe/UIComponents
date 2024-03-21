using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel.Design;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Generators.Configuration;
using UIComponents.Generators.Generators.FormButtons;
using UIComponents.Generators.Generators.Property.Inputs;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;
using UIComponents.Generators.Services;
using UIComponents.Generators.Services.Internal;
using UIComponents.Generators.Validators;

namespace UIComponents.Generators.Registrations;

public static class UICConfigure
{
    /// <summary>
    /// Add UIComponents to this project and configure the components
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IServiceCollection AddUIComponent(this IServiceCollection services, Action<UicConfigOptions> config)
    {
        return AddUIComponent(services, config, out var configOptions);
    }

    public static IServiceCollection AddUIComponent(this IServiceCollection services, Action<UicConfigOptions> config, out UicConfigOptions configOptions)
    {
        var configuration = new UicConfigOptions();
        services.TryAddScoped<IUIComponentService, UICService>();
        services.TryAddSingleton<IUICQuestionService, UICQuestionService>();
        services.TryAddSingleton<UicConfigOptions>(configuration);
        services.TryAddScoped<UICConfig>();

        services.TryAddSingleton<IUICStoredEvents, StoredEvents>();
        services.TryAddSingleton<IUICStoredComponents, StoredComponents>();
        services.TryAddScoped<IUICValidationService, UICValidationService>();

        config(configuration);

        if (!services.Where(x => x is IUICLanguageService).Any())
        {
            services.TryAddSingleton<IUICLanguageService, LanguageService>();
        }

        configOptions = configuration;
        return services;

    }

    /// <summary>
    /// Adding and registrating the default generators
    /// </summary>
    /// <remarks>
    /// Included sets: 
    /// <br><see cref="AddDefaultPropertyGenerators(UicConfigOptions, IServiceCollection)"/></br>
    /// <br><see cref="AddDefaultButtons(UicConfigOptions, IServiceCollection)"/></br>
    /// </remarks>
    public static UicConfigOptions AddDefaultGenerators(this UicConfigOptions configOptions, IServiceCollection serviceCollection)
    {
        configOptions.AddAndRegisterGenerator<UICPropTypeGenerator>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorInitialPartial>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorCard>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorForm>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorGroup>(serviceCollection);
        configOptions.AddDefaultPropertyGenerators(serviceCollection);
        configOptions.AddDefaultButtons(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorRequired>(serviceCollection);

        return configOptions;


    }

    public static UicConfigOptions AddDefaultPropertyGenerators(this UicConfigOptions configOptions, IServiceCollection serviceCollection)
    {
        configOptions.AddAndRegisterGenerator<UICGeneratorPropertyViewPermission>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorPropertyEditPermission>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorDataAnnotationValidators>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorInputGroupSpan>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorTooltip>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorInputTooltip>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorEnumSelectListItems>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorInputEditorTemplate>(serviceCollection);

        configOptions.AddAndRegisterGenerator<UICGeneratorInputGroup>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorLabel>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorInputText>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorInputMultiline>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorInputNumber>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorInputDateTime>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorInputSelectList>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorInputBool>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorInputThreeStateBool>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorInputTimespan>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorInputColor>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorInputRecurringDate>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorInputClassObject>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorInputList>(serviceCollection);

        return configOptions;
    }

    public static UicConfigOptions AddDefaultButtons(this UicConfigOptions configOptions, IServiceCollection serviceCollection)
    {
        configOptions.AddAndRegisterGenerator<UICGeneratorButtonCreate>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorButtonCancel>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorButtonDelete>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorButtonEditReadonly>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorButtonSave>(serviceCollection);
        configOptions.AddAndRegisterGenerator<UICGeneratorButtonToolbar>(serviceCollection);

        return configOptions;
    }

    public static UicConfigOptions AddDefaultValidators(this UicConfigOptions configOptions, IServiceCollection serviceCollection)
    {
        configOptions.AddAndRegisterValidator<UICValidatorRequired>(serviceCollection);

        configOptions.AddAndRegisterValidator<UICValidatorRangeAttributeInt>(serviceCollection);
        configOptions.AddAndRegisterValidator<UICValidatorRangeAttributeFloat>(serviceCollection);
        configOptions.AddAndRegisterValidator<UICValidatorRangeAttributeLong>(serviceCollection);
        configOptions.AddAndRegisterValidator<UICValidatorRangeAttributeDouble>(serviceCollection);
        configOptions.AddAndRegisterValidator<UICValidatorRangeAttributeDecimal>(serviceCollection);
        configOptions.AddAndRegisterValidator<UICValidatorRangeAttributeShort>(serviceCollection);
        configOptions.AddAndRegisterValidator<UICValidatorRangeAttributeDate>(serviceCollection);
        configOptions.AddAndRegisterValidator<UICValidatorRangeAttributeDateTime>(serviceCollection);
        configOptions.AddAndRegisterValidator<UICValidatorRangeAttributeTimeOnly>(serviceCollection);


        return configOptions;
    }


}
