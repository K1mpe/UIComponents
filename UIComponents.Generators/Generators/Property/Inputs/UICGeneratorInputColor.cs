using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputColor : UICGeneratorProperty
{

    public UICGeneratorInputColor()
    {
        HasExistingResult= false;
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        UICPropertyType = Abstractions.Attributes.UICPropertyType.HexColor;
    }
    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        var input = new UICInputColor(args.PropertyName)
        {
            Parent = args.CallCollection.Caller
        };
        input.Value = args.PropertyValue == null ? null : args.PropertyValue!.ToString();
        input.ValidationRequired = await args.Configuration.IsPropertyRequired(args, input)??false;

        await Task.Delay(0);
        return GeneratorHelper.Success<IUIComponent>(input, true);
    }
}
