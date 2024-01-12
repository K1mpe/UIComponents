using UIComponents.ComponentModels.Defaults;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.FormButtons;

public class UICGeneratorButtonCancel : UICGeneratorProperty
{
    public UICGeneratorButtonCancel()
    {
        RequiredCaller = UICGeneratorPropertyCallType.ButtonCancel;
        HasExistingResult = false;
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        var button = new UICButtonCancel();

        await Task.Delay(0);
        return GeneratorHelper.Success<IUIComponent>(button, true);
    }
}
