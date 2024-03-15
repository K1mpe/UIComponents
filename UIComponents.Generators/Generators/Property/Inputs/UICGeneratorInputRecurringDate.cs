using UIComponents.Abstractions.Models.RecurringDates;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputRecurringDate : UICGeneratorProperty
{
    public UICGeneratorInputRecurringDate()
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        HasExistingResult = false;
    }

    /// <summary>
    /// Priority must be lower than <see cref="UICGeneratorInputText"/> since the default <see cref="UICPropertyType"/> is string
    /// </summary>
    public override double Priority { get; set; } = 999;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (!args.PropertyType.IsAssignableTo(typeof(RecurringDate)))
            return GeneratorHelper.Next<IUIComponent>();

        var input = new UICInputRecurringDate(args.PropertyName)
        {
            Parent = args.CallCollection.Caller
        };

        input.Value = args.PropertyValue == null ? null : (RecurringDate)args.PropertyValue;

        return GeneratorHelper.Success<IUIComponent>(input, true);

    }
}
