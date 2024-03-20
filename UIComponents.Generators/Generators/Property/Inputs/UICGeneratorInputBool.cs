using Microsoft.Extensions.Logging;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputBool : UICGeneratorProperty
{
    public UICGeneratorInputBool(ILogger<UICGeneratorInputBool> logger) : base(logger)
    {
        UICPropertyType = Abstractions.Attributes.UICPropertyType.Boolean;
        HasExistingResult= false;
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        var input = new UICInputCheckbox(args.PropertyName)
        {
            Parent = args.CallCollection.Caller
        };
        input.Value = bool.Parse(args.PropertyValue!.ToString());
        input.Color = args.Options.CheckboxColor;
        input.Renderer= args.Options.CheckboxRenderer;
        return await Task.FromResult(GeneratorHelper.Success<IUIComponent>(input, true));
    }
}
