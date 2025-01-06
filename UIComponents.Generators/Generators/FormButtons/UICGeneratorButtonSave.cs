using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Extensions;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.FormButtons;

public class UICGeneratorButtonSave : UICGeneratorProperty
{
    public UICGeneratorButtonSave(ILogger<UICGeneratorButtonSave> logger) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.ButtonSave;
        HasExistingResult = false;
    }

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (existingResult != null)
            return GeneratorHelper.Next();
        if(args.ClassObject != null && args.Configuration.TryGetPermissionService(out var permissionService))
        {
            if (!await permissionService.CanEditObject(args.ClassObject))
            {
                _logger.LogDebug("No SaveButton is created because there is no permission to edit this object {0} ({1})", args.ClassObject.ToString(), args.ClassObject.GetType().Name);
                return GeneratorHelper.Success<IUIComponent>(null, false);
            }
                
        }

        if(args.CallCollection.Caller != null)
        {
            var form = args.CallCollection.Components.Where(c => c is UICForm).OfType<UICForm>().FirstOrDefault();
            if (form == null || form.TriggerSubmit() == null)
            {
                _logger.LogDebug("No SaveButton is created because no form was found");
                return GeneratorHelper.Next();
            }
        }
        

        var button = new UICButtonSave()
        {
            ButtonText = TranslationDefaults.ButtonSave,
        }.AddClass("btn-update");

        

        await Task.Delay(0);
        return GeneratorHelper.Success<IUIComponent>(button, true);
    }
}
