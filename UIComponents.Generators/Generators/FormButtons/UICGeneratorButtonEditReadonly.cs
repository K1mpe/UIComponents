using Microsoft.Extensions.Logging;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.FormButtons;

public class UICGeneratorButtonEditReadonly : UICGeneratorProperty
{
    public UICGeneratorButtonEditReadonly(ILogger<UICGeneratorButtonEditReadonly> logger) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.ButtonEditReadonly;
        HasExistingResult = false;
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        var button = new UICButtonEdit();
        button.ButtonSetReadonly.OnClick = new UICActionSetReadonly()
        {
            ShowEmptyInReadonly = !args.Options.HideEmptyInReadonly
        };
        await Task.Delay(0);
        return GeneratorHelper.Success<IUIComponent>(button, true);
    }
}
