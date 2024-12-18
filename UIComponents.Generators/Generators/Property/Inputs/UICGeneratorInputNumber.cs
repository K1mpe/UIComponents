using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputNumber : UICGeneratorProperty
{
    private readonly IUICValidationService _uicValidationService;
    public UICGeneratorInputNumber(ILogger<UICGeneratorInputNumber> logger, IUICValidationService uicValidationService) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        HasExistingResult = false;
        _uicValidationService = uicValidationService;
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.UICPropertyType != Abstractions.Attributes.UICPropertyType.Number && args.UICPropertyType != Abstractions.Attributes.UICPropertyType.Decimal)
            return GeneratorHelper.Next<IUIComponent>();

        var input = new UICInputNumber(args.PropertyName)
        {
            Parent = args.CallCollection.Caller
        };
        input.Value = args.PropertyValue==null?null: double.Parse(args.PropertyValue!.ToString());

        input.AllowDecimalValues = args.UICPropertyType == Abstractions.Attributes.UICPropertyType.Decimal;
        if (args.Options.CheckClientSideValidation)
        {
            input.ValidationRequired = await _uicValidationService.ValidatePropertyRequired(args.PropertyInfo, args.ClassObject);

            var propType = Nullable.GetUnderlyingType(args.PropertyType) ?? args.PropertyType;
            switch (propType.Name)
            {
                case nameof(Int16):
                    input.ValidationMinValue = ParseValue(await _uicValidationService.ValidatePropertyMinValue<short>(args.PropertyInfo, args.ClassObject));
                    input.ValidationMaxValue = ParseValue(await _uicValidationService.ValidatePropertyMaxValue<short>(args.PropertyInfo, args.ClassObject));
                    break;
                case nameof(Int32):
                    input.ValidationMinValue = ParseValue(await _uicValidationService.ValidatePropertyMinValue<int>(args.PropertyInfo, args.ClassObject));
                    input.ValidationMaxValue = ParseValue(await _uicValidationService.ValidatePropertyMaxValue<int>(args.PropertyInfo, args.ClassObject));
                    break;
                case nameof(Int64):
                    input.ValidationMinValue = ParseValue(await _uicValidationService.ValidatePropertyMinValue<long>(args.PropertyInfo, args.ClassObject));
                    input.ValidationMaxValue = ParseValue(await _uicValidationService.ValidatePropertyMaxValue<long>(args.PropertyInfo, args.ClassObject));
                    break;
                case nameof(Single):
                    input.ValidationMinValue = ParseValue(await _uicValidationService.ValidatePropertyMinValue<float>(args.PropertyInfo, args.ClassObject));
                    input.ValidationMaxValue = ParseValue(await _uicValidationService.ValidatePropertyMaxValue<float>(args.PropertyInfo, args.ClassObject));
                    break;
                case nameof(Double):
                    input.ValidationMinValue = ParseValue(await _uicValidationService.ValidatePropertyMinValue<double>(args.PropertyInfo, args.ClassObject));
                    input.ValidationMaxValue = ParseValue(await _uicValidationService.ValidatePropertyMaxValue<double>(args.PropertyInfo, args.ClassObject));
                    break;
                case nameof(Decimal):
                    input.ValidationMinValue = ParseValue(await _uicValidationService.ValidatePropertyMinValue<decimal>(args.PropertyInfo, args.ClassObject));
                    input.ValidationMaxValue = ParseValue(await _uicValidationService.ValidatePropertyMaxValue<decimal>(args.PropertyInfo, args.ClassObject));
                    break;
            }
        }
            
        decimal? ParseValue(object value)
        {
            if (value == null)
                return null;
            return decimal.Parse(value.ToString());
        }
        

        return GeneratorHelper.Success<IUIComponent>(input, true);
    }
}
