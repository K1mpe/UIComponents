using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.Property;

/// <summary>
/// Create a span field based on the <see cref="UICSpanAttribute"/>
/// </summary>
public class UICGeneratorInputGroupSpan : UICGeneratorProperty
{
    public UICGeneratorInputGroupSpan()
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyGroupSpan;
        HasExistingResult = false;
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.PropertyInfo == null)
            return await Task.FromResult(GeneratorHelper.Next<IUIComponent>());

        var spanAttr = args.PropertyInfo.GetCustomAttribute<UICSpanAttribute>();

        if(spanAttr == null && UICInheritAttribute.TryGetInheritPropertyInfo(args.PropertyInfo, out var inheritInfo))
        {
            spanAttr = inheritInfo.GetCustomAttribute<UICSpanAttribute>();
        }

        if (spanAttr == null)
            return await Task.FromResult(GeneratorHelper.Next<IUIComponent>());

        

        if (string.IsNullOrEmpty(spanAttr.TranslationModel.ResourceKey))
            spanAttr.TranslationModel.ResourceKey = TranslationDefaults.DefaultTooltipKey(args.PropertyInfo, args.UICPropertyType.Value);

        var span = new UICSpan(spanAttr.TranslationModel);
        return await Task.FromResult(GeneratorHelper.Success<IUIComponent>(span, true));
    }
}
