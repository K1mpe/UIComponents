using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Extensions;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.FormButtons;

public class UICGeneratorButtonCreate : UICGeneratorProperty
{
    public UICGeneratorButtonCreate(ILogger<UICGeneratorButtonCreate> logger) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.ButtonCreate;
        HasExistingResult= false;
    }
    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (existingResult != null)
            return GeneratorHelper.Next();

        if(args.ClassObject != null && args.Configuration.TryGetPermissionService(out var permissionService))
        {
            if (!await permissionService!.CanCreateType(args.ClassObject.GetType()))
            {
                _logger.LogDebug("No Createbutton is created because there is no permission to create for type {0}", args.ClassObject.GetType().Name);
                return GeneratorHelper.Success<IUIComponent>(null, false);
            }
        }
        if(args.CallCollection.Caller != null)
        {
            var form = args.CallCollection.Components.Where(c => c is UICForm).OfType<UICForm>().FirstOrDefault();
            if (form == null || form.TriggerSubmit() == null)
            {
                _logger.LogDebug("No Createbutton is created because no form was found");
                return GeneratorHelper.Next();
            }
        }
        

        var button = new UICButtonSave()
        {
            ButtonText = TranslationDefaults.ButtonCreate,
            PrependButtonIcon = IconDefaults.Create?.Invoke()
        }.AddClass("btn-create");

        await Task.Delay(0);
        return GeneratorHelper.Success<IUIComponent>(button, true);
    }
}
