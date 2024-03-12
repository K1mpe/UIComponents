using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.Property;


/// <summary>
/// Create a tooltip based on the <see cref="UICTooltipAttribute"/>
/// </summary>
public class UICGeneratorTooltip : UICGeneratorBase<UICPropertyArgs, Translatable>
{

    public UICGeneratorTooltip()
    {
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<Translatable>> GetResponseAsync(UICPropertyArgs args, Translatable? existingResult)
    {
        if (args.PropertyInfo == null)
            return GeneratorHelper.Next<Translatable>();

        if (args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.PropertyTooltip)
            return GeneratorHelper.Next<Translatable>();

        if (existingResult != null)
            return GeneratorHelper.Next<Translatable>();

        var tooltipAttr = args.PropertyInfo.GetCustomAttribute<UICTooltipAttribute>();

        if(tooltipAttr == null && UICInheritAttribute.TryGetInheritPropertyInfo(args.PropertyInfo, out var inheritInfo)) 
            tooltipAttr = inheritInfo.GetCustomAttribute<UICTooltipAttribute>();

        if (tooltipAttr != null)
        {
            if (string.IsNullOrEmpty(tooltipAttr.TranslationModel.ResourceKey))
                tooltipAttr.TranslationModel.ResourceKey = TranslationDefaults.DefaultTooltipKey(args.PropertyInfo, args.UICPropertyType.Value);

            return GeneratorHelper.Success<Translatable>(tooltipAttr.TranslationModel, true);
        }

        await Task.Delay(0);
        return GeneratorHelper.Next<Translatable>();
    }
}
