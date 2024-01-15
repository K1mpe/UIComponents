using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputTimespan : UICGeneratorProperty
{

    public UICGeneratorInputTimespan()
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        UICPropertyType = Abstractions.Attributes.UICPropertyType.Timespan;
        HasExistingResult= false;
    }
    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        var input = new UICInputTimespan(args.PropertyName);
        input.Value = args.PropertyValue==null?null: (TimeSpan)args.PropertyValue;

        input.ValidationRequired = await args.Configuration.IsPropertyRequired(args, input)??false;

        return GeneratorHelper.Success<IUIComponent>(input, true);
    }
}
