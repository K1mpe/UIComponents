using System.Reflection;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property;


/// <summary>
/// Create a tooltip based on the <see cref="UICTooltipAttribute"/>
/// </summary>
public class UICGeneratorTooltip : UICGeneratorBase<UICPropertyArgs, ITranslationModel>
{

    public UICGeneratorTooltip()
    {
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<ITranslationModel>> GetResponseAsync(UICPropertyArgs args, ITranslationModel? existingResult)
    {
        if (args.PropertyInfo == null)
            return GeneratorHelper.Next<ITranslationModel>();

        if (args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.PropertyTooltip)
            return GeneratorHelper.Next<ITranslationModel>();

        if (existingResult != null)
            return GeneratorHelper.Next<ITranslationModel>();

        var tooltipAttr = args.PropertyInfo.GetCustomAttribute<UICTooltipAttribute>();
        if (tooltipAttr != null)
        {
            return GeneratorHelper.Success<ITranslationModel>(tooltipAttr.TranslationModel, true);
        }

        await Task.Delay(0);
        return GeneratorHelper.Next<ITranslationModel>();
    }
}
