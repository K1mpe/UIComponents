using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputTimespan : UICGeneratorProperty
{

    private readonly IUICValidationService _validationService;
    public UICGeneratorInputTimespan(ILogger<UICGeneratorInputTimespan> logger, IUICValidationService validationService) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        UICPropertyType = Abstractions.Attributes.UICPropertyType.TimeSpan;
        HasExistingResult = false;
        _validationService = validationService;
    }
    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        var input = new UICInputTimespan(args.PropertyName)
        {
            Parent = args.CallCollection.Caller
        };
        input.Value = args.PropertyValue==null?null: (TimeSpan)args.PropertyValue;

        input.ValidationRequired = await _validationService.ValidatePropertyRequired(args.PropertyInfo, args.ClassObject);
        input.ValidationMinValue = await _validationService.ValidatePropertyMinValue<TimeSpan>(args.PropertyInfo, args.ClassObject);
        input.ValidationMaxValue = await _validationService.ValidatePropertyMaxValue<TimeSpan>(args.PropertyInfo, args.ClassObject);


        return GeneratorHelper.Success<IUIComponent>(input, true);
    }
}
