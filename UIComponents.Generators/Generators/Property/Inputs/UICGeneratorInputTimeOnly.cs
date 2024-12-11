using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputTimeOnly : UICGeneratorProperty
{

    private readonly IUICValidationService _validationService;

    public UICGeneratorInputTimeOnly(ILogger<UICGeneratorInputTimeOnly> logger, IUICValidationService validationService) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        HasExistingResult = false;
        _validationService = validationService;
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.UICPropertyType != Abstractions.Attributes.UICPropertyType.TimeOnly)
            return GeneratorHelper.Next<IUIComponent>();

        var input = new UICInputTime(args.PropertyName)
        {
            Parent = args.CallCollection.Caller
        };
        input.Precision = args.Options.TimeOnlyPrecision;

        if (args.PropertyValue is DateTime || args.PropertyValue is DateTime?)
        {
            ParseDateTime(args.PropertyValue, converted => input.Value = converted);
        }
        else
        {
            input.Value = args.PropertyValue == null ? null : TimeOnly.Parse(args.PropertyValue.ToString());
        }


        if (args.Options.CheckClientSideValidation)
        {
            input.ValidationRequired = await _validationService.ValidatePropertyRequired(args.PropertyInfo, args.ClassObject);

            var propType = Nullable.GetUnderlyingType(args.PropertyType) ?? args.PropertyType;
            switch (propType.Name)
            {
                case nameof(TimeOnly):
                    input.ValidationMinTime = await _validationService.ValidatePropertyMinValue<TimeOnly>(args.PropertyInfo, args.ClassObject);
                    input.ValidationMaxTime = await _validationService.ValidatePropertyMaxValue<TimeOnly>(args.PropertyInfo, args.ClassObject);
                    break;
                case nameof(DateTime):
                    var min = (await _validationService.ValidatePropertyMinValue<DateTime>(args.PropertyInfo, args.ClassObject));
                    var max = (await _validationService.ValidatePropertyMaxValue<DateTime>(args.PropertyInfo, args.ClassObject));
                    ParseDateTime(min, c => input.ValidationMinTime = c);
                    ParseDateTime(max, c => input.ValidationMaxTime = c);
                    break;
            }
        }
            


        var precisionAttr = args.PropertyInfo.GetInheritAttribute<UICPrecisionTimeAttribute>();
        if (precisionAttr != null)
        {
            input.Precision = precisionAttr.Precision;
        }


        return GeneratorHelper.Success<IUIComponent>(input, true);

    }

    /// <summary>
    /// If the object has a value and can be parsed to DateTime, convert the object to a timeonly and run the action.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="parseAction"></param>
    private void ParseDateTime(object obj, Action<TimeOnly> parseAction)
    {
        if (obj == null)
            return;

        if (obj is TimeOnly || obj is TimeOnly?)
        {
            var t1 = (TimeOnly)obj;
            parseAction(t1);
            return;
        }

        if (obj is DateTime || obj is DateTime?)
        {
            var dateTime = DateTime.Parse(obj.ToString());
            var t2 = new TimeOnly(dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond);
            parseAction(t2);
            return;
        }

        if (TimeOnly.TryParse(obj.ToString(), out var t3))
        {
            parseAction(t3);
            return;
        }
    }
}
