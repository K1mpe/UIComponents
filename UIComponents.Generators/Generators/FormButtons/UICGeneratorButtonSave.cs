using UIComponents.Generators.Helpers;

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
            if (!await permissionService.CanEditObject(args.ClassObject))
                return GeneratorHelper.Success<IUIComponent>(null, false);
        }

        var form = args.CallCollection.Components.Where(c => c is UICForm).OfType<UICForm>().FirstOrDefault();
        if (form == null || form.TriggerSubmit() == null)
            return GeneratorHelper.Next();

        var button = new UICButtonSave()
        {
            ButtonText = TranslationDefaults.ButtonSave,
        };

        

        await Task.Delay(0);
        return GeneratorHelper.Success<IUIComponent>(button, true);
    }
}
