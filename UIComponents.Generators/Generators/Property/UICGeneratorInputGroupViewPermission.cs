using Microsoft.Extensions.Logging;
using UIComponents.Generators.Generators.Property.Inputs;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Models.UICGeneratorResponses;

namespace UIComponents.Generators.Generators.Property;


/// <summary>
/// Check if the current user has permission to view this object or property of the object
/// </summary>
public class UICGeneratorPropertyViewPermission : UICGeneratorProperty
{
    public UICGeneratorPropertyViewPermission(ILogger<UICGeneratorInputThreeStateBool> logger) : base(logger)
    {

    }
    public override double Priority { get; set; } = 1;


    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (!args.Options.CheckReadPermissions)
            return GeneratorHelper.Next();

        if (!args.Configuration.TryGetPermissionService(out var permissionService))
            return new UICGeneratorResponseNext<IUIComponent>();

        if (!await permissionService!.CanViewObject(args.ClassObject))
            return new UICGeneratorResponseSuccess<IUIComponent>(null, false);

      
        if(args.PropertyInfo != null)
        {
            if(UICInheritAttribute.TryGetInheritPropertyInfo(args.PropertyInfo, out var inheritInfo))
            {
                try
                {
                    var instance = Activator.CreateInstance(inheritInfo.ReflectedType);
                    if (!await permissionService!.CanViewObject(instance))
                        return new UICGeneratorResponseSuccess<IUIComponent>(null, false);

                    if (!await permissionService.CanViewProperty(instance, inheritInfo.Name))
                        return new UICGeneratorResponseSuccess<IUIComponent>(null, false);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error Creating Inherit permissions");
                }
                
            }
            if(!await permissionService.CanViewProperty(args.ClassObject, args.PropertyName!))
                return new UICGeneratorResponseSuccess<IUIComponent>(null, false);
        }
        return new UICGeneratorResponseNext<IUIComponent>();
    }
}
