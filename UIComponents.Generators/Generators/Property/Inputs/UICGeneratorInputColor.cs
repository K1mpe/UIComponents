using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputColor : UICGeneratorProperty
{
    private readonly IUICValidationService _validationService;
    public UICGeneratorInputColor(ILogger<UICGeneratorInputColor> logger, IUICValidationService validationService) : base(logger)
    {
        HasExistingResult = false;
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        UICPropertyType = Abstractions.Attributes.UICPropertyType.HexColor;
        _validationService = validationService;
    }
    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        var input = new UICInputColor(args.PropertyName)
        {
            Parent = args.CallCollection.Caller
        };
        input.Value = args.PropertyValue == null ? null : args.PropertyValue!.ToString();
        if(args.Options.CheckClientSideValidation) 
            input.ValidationRequired = await _validationService.ValidatePropertyRequired(args.PropertyInfo, args.ClassObject);

        await Task.Delay(0);
        return GeneratorHelper.Success<IUIComponent>(input, true);
    }
}
