using UIComponents.Abstractions.Models;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property.Inputs;


/// <summary>
/// If a input does not have a tooltip yet, run all tooltipgenerators
/// </summary>
public class UICGeneratorInputTooltip : UICGeneratorProperty
{

    public UICGeneratorInputTooltip()
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        HasExistingResult = true;
    }

    public override double Priority { get; set; } = 1005;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if(existingResult is UICInput input && input.Tooltip == null)
        {
            input.Tooltip = await args.Configuration.GetToolTipAsync(args, input);
            return GeneratorHelper.Success<IUIComponent>(input, true);
        }
        await Task.Delay(0);
        return GeneratorHelper.Next<IUIComponent>();
    }
}
