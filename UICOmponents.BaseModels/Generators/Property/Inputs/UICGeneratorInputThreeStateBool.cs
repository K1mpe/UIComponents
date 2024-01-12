using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputThreeStateBool : UICGeneratorProperty
{
    public UICGeneratorInputThreeStateBool()
    {
        UICPropertyType = Abstractions.Attributes.UICPropertyType.ThreeStateBoolean;
        HasExistingResult = false;
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        var input = new UICInputCheckboxThreeState(args.PropertyName);
        input.Value = args.PropertyValue==null?null:bool.Parse(args.PropertyValue.ToString());
        input.Color = args.Options.CheckboxColor;
        input.Renderer = args.Options.CheckboxRenderer;
        return await Task.FromResult(GeneratorHelper.Success<IUIComponent>(input, true));
    }
}
