using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Abstractions.Models;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property.Inputs;

/// <summary>
/// Use the <see cref="IPermissionCurrentUserService"/> to make <see cref="UICInput"/> readonly if access denied
/// </summary>
public class UICGeneratorPropertySetReadonly : UICGeneratorProperty
{
    private readonly IUICValidationService _validationService;

    public UICGeneratorPropertySetReadonly(ILogger<UICGeneratorInputThreeStateBool> logger, IUICValidationService validationService) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        HasExistingResult = true;
        _validationService = validationService;
    }

    public override double Priority { get; set; } = 1005;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {

        if(existingResult is UICInput input && !input.Readonly)
        {
            if(args.Options.FormReadonly)
            {
                input.Readonly = true;
                return GeneratorHelper.Success(existingResult, true);
            }

            if (!args.Options.CheckWritePermissions)
                return GeneratorHelper.Next();
            input.Readonly = await _validationService.ValidatePropertyReadonly(args.PropertyInfo, args.ClassObject);

            if(!input.Readonly &&  args.Configuration.TryGetPermissionService(out var permissionService))
            {
                input.Readonly = !(await permissionService!.CanEditObject(args.ClassObject!) && await permissionService.CanEditProperty(args.ClassObject, args.PropertyName));
                if(!input.Readonly && UICInheritAttribute.TryGetInheritPropertyInfo(args.PropertyInfo, out var inherit))
                {
                    var inheritInstance = Activator.CreateInstance(inherit.ReflectedType);
                    foreach(var property in args.PropertyInfo.ReflectedType.GetProperties())
                    {
                        if (!property.CanRead)
                            continue;

                        if (UICInheritAttribute.TryGetInheritPropertyInfo(property, out var x) && x.DeclaringType == inherit.DeclaringType && x.CanWrite)
                            x.SetValue(inheritInstance, property.GetValue(args.ClassObject));
                    }

                    input.Readonly = !(await permissionService!.CanEditObject(inheritInstance) && await permissionService!.CanEditProperty(inheritInstance, inherit.Name));
                }
            }
            return GeneratorHelper.Success<IUIComponent>(input, true);
        }
        return GeneratorHelper.Next<IUIComponent>();
    }
}
