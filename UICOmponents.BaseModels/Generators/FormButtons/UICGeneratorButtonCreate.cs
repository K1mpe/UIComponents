using UIComponents.ComponentModels.Defaults;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.FormButtons;

public class UICGeneratorButtonCreate : UICGeneratorProperty
{
    public UICGeneratorButtonCreate()
    {
        RequiredCaller = UICGeneratorPropertyCallType.ButtonCreate;
        HasExistingResult= false;
    }
    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if(args.Configuration.TryGetPermissionService(out var permissionService))
        {
            if (!await permissionService!.CanCreate(args.ClassObject))
                return GeneratorHelper.Success<IUIComponent>(null, false);
        }

        var button = new UICButtonSave()
        {
            ButtonText = TranslationDefaults.ButtonCreate,
            OnClick = new UICActionSubmit(args.Options.FormPostUrl ?? $"/{args.ClassObject.GetType().Name}/Create")
            {
                OnSuccess = new UICActionCloseCard()
                {
                    OnFailed = new UICActionNavigate($"/{args.ClassObject.GetType().Name}/Details/${{result.Id}}")
                }
            }
        };

        await Task.Delay(0);
        return GeneratorHelper.Success<IUIComponent>(button, true);
    }
}
