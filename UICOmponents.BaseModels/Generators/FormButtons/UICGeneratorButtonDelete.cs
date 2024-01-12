using UIComponents.ComponentModels.Defaults;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.FormButtons;

public class UICGeneratorButtonDelete : UICGeneratorProperty
{
    public UICGeneratorButtonDelete()
    {
        RequiredCaller = UICGeneratorPropertyCallType.ButtonDelete;
        HasExistingResult = false;
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if(args.Configuration.TryGetPermissionService(out var permissionService))
        {
            if (!await permissionService!.CanDelete(args.ClassObject))
                return GeneratorHelper.Success<IUIComponent>(null, false);
        }
        if(args.ClassObject is IDbEntity dbEntity)
        {
            var button = new UICButtonDelete(args.ClassObject.GetType().Name, dbEntity.Id);

            return GeneratorHelper.Success<IUIComponent>(button, true);
        }
        return GeneratorHelper.Next<IUIComponent>();
    }
}
