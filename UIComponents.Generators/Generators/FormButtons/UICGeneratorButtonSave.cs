using UIComponents.Generators.Helpers;
using UIComponents.Models.Defaults;

namespace UIComponents.Generators.Generators.FormButtons;

public class UICGeneratorButtonSave : UICGeneratorProperty
{
    public UICGeneratorButtonSave()
    {
        RequiredCaller = UICGeneratorPropertyCallType.ButtonSave;
        HasExistingResult = false;
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if(args.Configuration.TryGetPermissionService(out var permissionService))
        {
            if (!await permissionService.CanEdit(args.ClassObject))
                return GeneratorHelper.Success<IUIComponent>(null, false);
        }


        var submitAction = new UICActionSubmit(args.Options.FormPostUrl ?? $"/{args.ClassObject.GetType().Name}/Update")
        {
            OnSuccess = new UICActionCloseCard()
        };

        if (args.ClassObject is IDbEntity dbEntity)
            submitAction.AdditionalPost = new { Id = dbEntity.Id };

        var button = new UICButtonSave()
        {
            ButtonText = TranslationDefaults.ButtonCreate,
            OnClick = submitAction
        };

        

        await Task.Delay(0);
        return GeneratorHelper.Success<IUIComponent>(button, true);
    }
}
