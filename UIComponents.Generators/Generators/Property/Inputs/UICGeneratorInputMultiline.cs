
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputMultiline : UICGeneratorProperty
{
    public UICGeneratorInputMultiline()
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        UICPropertyType = Abstractions.Attributes.UICPropertyType.MultilineText;
        HasExistingResult = false;
    }
    public override double Priority { get; set; }

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        var input = new UICInputMultiline(args.PropertyName)
        {
            Parent = args.CallCollection.Caller
        };
        input.Value = args.PropertyValue?.ToString()?? string.Empty;

        if (args.Configuration.TryGetPermissionService(out var permissionService))
            input.Readonly = !(await permissionService!.CanEditObject(args.ClassObject!) && await permissionService.CanEditProperty(args.ClassObject, args.PropertyName));



        input.ValidationRequired = await args.Configuration.IsPropertyRequired(args, input) ?? false;

        return GeneratorHelper.Success<IUIComponent>(input, true);

    }
}
