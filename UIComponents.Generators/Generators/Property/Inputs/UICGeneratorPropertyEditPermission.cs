using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;
using UIComponents.Models.Abstract;

namespace UIComponents.Generators.Generators.Property.Inputs;

/// <summary>
/// Use the <see cref="IPermissionCurrentUserService"/> to make <see cref="UICInput"/> readonly if access denied
/// </summary>
public class UICGeneratorPropertyEditPermission : UICGeneratorProperty
{
    public UICGeneratorPropertyEditPermission()
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        HasExistingResult = true;
    }

    public override double Priority { get; set; } = 1005;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if(existingResult is UICInput input && !input.Readonly)
        {
            if(args.Configuration.TryGetPermissionService(out var permissionService))
            {
                input.Readonly = await permissionService!.CanEditProperty(args.ClassObject, args.PropertyName!);
                return GeneratorHelper.Success<IUIComponent>(input, true);
            }
        }
        return GeneratorHelper.Next<IUIComponent>();
    }
}
