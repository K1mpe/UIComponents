using Microsoft.Extensions.Logging;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputDateTime : UICGeneratorProperty
{
    public UICGeneratorInputDateTime(ILogger<UICGeneratorInputDateTime> logger) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        HasExistingResult = false;
    }

    public override double Priority { get; set; }

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.UICPropertyType != Abstractions.Attributes.UICPropertyType.DateOnly && args.UICPropertyType != Abstractions.Attributes.UICPropertyType.DateTime)
            return GeneratorHelper.Next<IUIComponent>();

        var input = new UICInputDatetime(args.PropertyName)
        {
            Parent = args.CallCollection.Caller
        };
        input.Precision = args.Options.DatetimePrecision;

        if (args.UICPropertyType == Abstractions.Attributes.UICPropertyType.DateOnly)
            input.Precision = UICDatetimeStep.Date;

        input.Value = args.PropertyValue == null ? null : DateTime.Parse(args.PropertyValue.ToString());

        input.ValidationRequired = await args.Configuration.IsPropertyRequired(args, input)??false;

        return GeneratorHelper.Success<IUIComponent>(input, true);

    }
}
