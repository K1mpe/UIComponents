using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;
using UIComponents.Models.Models;
using UIComponents.Models.Models.Actions;
using UIComponents.Models.Models.Buttons;

namespace UIComponents.Generators.Generators.Property;

public class UICGeneratorForm : UICGeneratorProperty
{
    public UICGeneratorForm()
    {
        RequiredCaller = UICGeneratorPropertyCallType.ClassObject;
        HasExistingResult= false;
    }

    public override double Priority { get; set; } = 1;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.Options.FormReadonly)
            return GeneratorHelper.Next<IUIComponent>();

        if(args.CallCollection.Components.Any(x=>x.GetType().IsAssignableTo(typeof(UICForm))))
            return GeneratorHelper.Next<IUIComponent>();

        if (args.Configuration.TryGetPermissionService(out var permissionService))
            if (!args.Options.ReplaceSaveButtonWithCreateButton && !await permissionService.CanEdit(args.ClassObject!))
                return GeneratorHelper.Next<IUIComponent>();

        var form = new UICForm();

        var submit= new UICActionGetPost(UICActionGetPost.ActionTypeEnum.Post, args.ClassObject.GetType().Name, args.Options.ReplaceSaveButtonWithCreateButton ? "Create" : "Update") 
        {
            GetVariableData = form.TriggerGetValue()
        };
        if(args.ClassObject is IDbEntity dbEntity)
            submit.Data = new {Id = dbEntity.Id};
        form.Submit = submit;

        var newCC = new UICCallCollection(UICGeneratorPropertyCallType.ClassObject, form, args.CallCollection);
        form.Add(await args.Configuration.GetChildComponentAsync(args.ClassObject, args.PropertyInfo, args.Options, newCC));
        form.Parent = args.CallCollection.Caller;

        var toolbarCC = new UICCallCollection(UICGeneratorPropertyCallType.ButtonToolbar, form, args.CallCollection);
        form.Add(await args.Configuration.GetChildComponentAsync(args.ClassObject, args.PropertyInfo, args.Options, toolbarCC));
        

        return GeneratorHelper.Success<IUIComponent>(form, true);

    }
}
