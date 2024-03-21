using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputDateTime : UICGeneratorProperty
{
    private readonly IUICValidationService _validationService;

    public UICGeneratorInputDateTime(ILogger<UICGeneratorInputDateTime> logger, IUICValidationService validationService) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        HasExistingResult = false;
        _validationService = validationService;
    }

    public override double Priority { get; set; } = 1000;

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

        input.ValidationRequired = await _validationService.ValidatePropertyRequired(args.PropertyInfo, args.ClassObject);

        var propType = Nullable.GetUnderlyingType(args.PropertyType) ?? args.PropertyType;
        switch (propType.Name)
        {
            case nameof(DateTime):
                input.ValidationMinimumDate = await _validationService.ValidatePropertyMinValue<DateTime>(args.PropertyInfo, args.ClassObject);
                input.ValidationMaximumDate = await _validationService.ValidatePropertyMaxValue<DateTime>(args.PropertyInfo, args.ClassObject);
                break;
            case nameof(DateOnly):
                input.ValidationMinimumDate = (await _validationService.ValidatePropertyMinValue<DateOnly>(args.PropertyInfo, args.ClassObject))?.ToDateTime(new())??null;
                input.ValidationMaximumDate = (await _validationService.ValidatePropertyMaxValue<DateOnly>(args.PropertyInfo, args.ClassObject))?.ToDateTime(new()) ?? null;
                break;
        }

        return GeneratorHelper.Success<IUIComponent>(input, true);

    }
}
