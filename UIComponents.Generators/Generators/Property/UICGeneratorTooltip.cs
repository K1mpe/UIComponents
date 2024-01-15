using System.Reflection;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property;


/// <summary>
/// Create a tooltip based on the <see cref="UICTooltipAttribute"/>
/// </summary>
public class UICGeneratorTooltip : UICGeneratorBase<UICPropertyArgs, ITranslateable>
{

    public UICGeneratorTooltip()
    {
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<ITranslateable>> GetResponseAsync(UICPropertyArgs args, ITranslateable? existingResult)
    {
        if (args.PropertyInfo == null)
            return GeneratorHelper.Next<ITranslateable>();

        if (args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.PropertyTooltip)
            return GeneratorHelper.Next<ITranslateable>();

        if (existingResult != null)
            return GeneratorHelper.Next<ITranslateable>();

        var tooltipAttr = args.PropertyInfo.GetCustomAttribute<UICTooltipAttribute>();
        if (tooltipAttr != null)
        {
            return GeneratorHelper.Success<ITranslateable>(tooltipAttr.TranslationModel, true);
        }

        await Task.Delay(0);
        return GeneratorHelper.Next<ITranslateable>();
    }
}
