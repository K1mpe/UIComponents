using UIComponents.Abstractions.Models;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

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
        if (!args.Options.CheckWritePermissions)
            return GeneratorHelper.Next();

        if(existingResult is UICInput input && !input.Readonly)
        {
            if(args.Configuration.TryGetPermissionService(out var permissionService))
            {
                input.Readonly = !await permissionService.CanEditProperty(args.ClassObject, args.PropertyName);
                if(!input.Readonly && UICInheritAttribute.TryGetInheritPropertyInfo(args.PropertyInfo, out var inherit))
                {
                    var inheritInstance = Activator.CreateInstance(inherit.DeclaringType);
                    foreach(var property in args.PropertyInfo.DeclaringType.GetProperties())
                    {
                        if (UICInheritAttribute.TryGetInheritPropertyInfo(property, out var x) && x.DeclaringType == inherit.DeclaringType)
                            x.SetValue(inheritInstance, property.GetValue(args.ClassObject));
                    }

                    input.Readonly = !await permissionService!.CanEditProperty(inheritInstance, inherit.Name);
                }
                return GeneratorHelper.Success<IUIComponent>(input, true);
            }
        }
        return GeneratorHelper.Next<IUIComponent>();
    }
}
