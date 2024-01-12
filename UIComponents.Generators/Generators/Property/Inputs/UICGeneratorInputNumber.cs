using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputNumber : UICGeneratorProperty
{

    public UICGeneratorInputNumber()
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        HasExistingResult = false;
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.UICPropertyType != Abstractions.Attributes.UICPropertyType.Number || args.UICPropertyType != Abstractions.Attributes.UICPropertyType.Decimal)
            return GeneratorHelper.Next<IUIComponent>();

        var input = new UICInputNumber(args.PropertyName);
        input.Value = args.PropertyValue==null?null: double.Parse(args.PropertyValue!.ToString());

        input.AllowDecimalValues = args.UICPropertyType == Abstractions.Attributes.UICPropertyType.Decimal;
        input.ValidationRequired = await args.Configuration.IsPropertyRequired(args, input) ?? false;

        return GeneratorHelper.Success<IUIComponent>(input, true);
    }
}
