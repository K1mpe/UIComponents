using UIComponents.Generators.Helpers;
using UIComponents.Generators.Models.UICGeneratorResponses;

namespace UIComponents.Generators.Generators.Property;


/// <summary>
/// Check if the current user has permission to view this object or property of the object
/// </summary>
public class UICGeneratorPropertyViewPermission : UICGeneratorProperty
{
    public override double Priority { get; set; } = 1;


    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (!args.Options.CheckReadPermissions)
            return GeneratorHelper.Next();

        if (!args.Configuration.TryGetPermissionService(out var permissionService))
            return new UICGeneratorResponseNext<IUIComponent>();

        if (!await permissionService!.CanView(args.ClassObject))
            return new UICGeneratorResponseSuccess<IUIComponent>(null, false);

      
        if(args.PropertyInfo != null)
        {
            if(UICInheritAttribute.TryGetInheritPropertyInfo(args.PropertyInfo, out var inheritInfo))
            {
                if (!await permissionService!.CanView(inheritInfo.DeclaringType))
                    return new UICGeneratorResponseSuccess<IUIComponent>(null, false);

                if (!await permissionService.CanViewProperty(inheritInfo.DeclaringType, inheritInfo.Name))
                    return new UICGeneratorResponseSuccess<IUIComponent>(null, false);
            }
            if(!await permissionService.CanViewProperty(args.ClassObject, args.PropertyName!))
                return new UICGeneratorResponseSuccess<IUIComponent>(null, false);
        }
        return new UICGeneratorResponseNext<IUIComponent>();
    }
}
