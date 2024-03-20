using Microsoft.Extensions.Logging;
using System.Collections;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputList : UICGeneratorProperty
{
    public UICGeneratorInputList(ILogger<UICGeneratorInputList> logger) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        HasExistingResult = false;

    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.PropertyType == null)
            return GeneratorHelper.Next<IUIComponent>();


        if (args.PropertyValue == null)
            return GeneratorHelper.Next<IUIComponent>();



        if (args.PropertyValue is IEnumerable enumerable)
        {
            throw new NotImplementedException($"Generator for IEnumerables is not yet implemented");
            var input = new UICInputList(args.PropertyName)
            {
                Parent = args.CallCollection.Caller
            };
            input.ItemType = args.PropertyType.GetGenericArguments()[0];

            var type = input.ItemType;
            if (false)
            {

            }
            else if (type.IsAssignableTo(typeof(bool?)))
                input.SingleInstanceInput = new UICInputCheckboxThreeState(string.Empty);
            else if (type.IsAssignableTo(typeof(bool)))
                input.SingleInstanceInput = new UICInputCheckbox(string.Empty);
           


            if (input.SingleInstanceInput == null)
                throw new NotImplementedException($"There is no input available for {input.ItemType.Name} as a list");

            return GeneratorHelper.Success<IUIComponent>(input, true);
        }

        return GeneratorHelper.Next<IUIComponent>();
    }
}
