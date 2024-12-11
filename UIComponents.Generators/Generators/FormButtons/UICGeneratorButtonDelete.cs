using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.FormButtons;

public class UICGeneratorButtonDelete : UICGeneratorProperty
{
    public UICGeneratorButtonDelete(ILogger<UICGeneratorButtonDelete> logger) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.ButtonDelete;
        HasExistingResult = false;
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if(args.Configuration.TryGetPermissionService(out var permissionService))
        {
            if (!await permissionService!.CanDeleteObject(args.ClassObject))
                return GeneratorHelper.Success<IUIComponent>(null, false);
        }
        if (args.Options.PostFullModelOnDelete)
        {
            var button = new UICButtonDelete(args.ClassObject.GetType(), args.ClassObject);

            return GeneratorHelper.Success<IUIComponent>(button, true);
        }
        else
        {
            if (args.ClassObject is IDbEntity dbEntity)
            {
                var button = new UICButtonDelete(args.ClassObject.GetType(), dbEntity.Id);

                return GeneratorHelper.Success<IUIComponent>(button, true);
            }
        }
        return GeneratorHelper.Next<IUIComponent>();
    }
}
