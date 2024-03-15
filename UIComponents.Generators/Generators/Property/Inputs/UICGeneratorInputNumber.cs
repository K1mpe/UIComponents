using System.ComponentModel.DataAnnotations;
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
        if (args.UICPropertyType != Abstractions.Attributes.UICPropertyType.Number && args.UICPropertyType != Abstractions.Attributes.UICPropertyType.Decimal)
            return GeneratorHelper.Next<IUIComponent>();

        var input = new UICInputNumber(args.PropertyName)
        {
            Parent = args.CallCollection.Caller
        };
        input.Value = args.PropertyValue==null?null: double.Parse(args.PropertyValue!.ToString());

        input.AllowDecimalValues = args.UICPropertyType == Abstractions.Attributes.UICPropertyType.Decimal;
        input.ValidationRequired = await args.Configuration.IsPropertyRequired(args, input) ?? false;

        var rangeAttr = args.PropertyInfo.GetCustomAttribute<RangeAttribute>();
        if(rangeAttr != null)
        {
            try
            {
                if ((int)rangeAttr.Minimum > int.MinValue)
                    input.ValidationMinValue = (int)rangeAttr.Minimum;
            }
            catch { }
            try
            {
                if ((int)rangeAttr.Maximum < int.MaxValue)
                    input.ValidationMaxValue = (int)rangeAttr.Maximum;
            }
            catch { }

            
        }

        return GeneratorHelper.Success<IUIComponent>(input, true);
    }
}
