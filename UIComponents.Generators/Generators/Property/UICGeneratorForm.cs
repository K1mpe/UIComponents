using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;
using UIComponents.Generators.Generators.Property.Inputs;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;
using UIComponents.Models.Models;
using UIComponents.Models.Models.Actions;
using UIComponents.Models.Models.Buttons;
using UIComponents.Models.Models.Card;

namespace UIComponents.Generators.Generators.Property;

public class UICGeneratorForm : UICGeneratorProperty
{
    public UICGeneratorForm(ILogger<UICGeneratorInputThreeStateBool> logger) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.ClassObject;
        HasExistingResult= false;
    }

    public override double Priority { get; set; } = 1;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.Options.NoForm)
            return GeneratorHelper.Next<IUIComponent>();

        if (args.Options.FormReadonly)
            return GeneratorHelper.Next<IUIComponent>();

        if(args.CallCollection.Components.Any(x=>x.GetType().IsAssignableTo(typeof(UICForm))))
            return GeneratorHelper.Next<IUIComponent>();

        if (args.Configuration.TryGetPermissionService(out var permissionService))
            if (!args.Options.ReplaceSaveButtonWithCreateButton && !await permissionService.CanEditObject(args.ClassObject!))
                return GeneratorHelper.Next<IUIComponent>();

        var form = new UICForm()
        {
            Parent = args.CallCollection.Caller
        };
        if (args.Options.PostForm == null)
        {
            var submit = new UICActionGetPost(UICActionGetPost.ActionTypeEnum.Post, args.ClassObject.GetType().Name, args.Options.ReplaceSaveButtonWithCreateButton ? "Create" : "Update")
            {
                GetVariableData = form.TriggerGetValue()
            };
            
            form.Submit = submit;
        }
        else
        {
            form.Submit = args.Options.PostForm;
        }
        if(form.Submit is UICActionGetPost getPost)
        {
            if (args.ClassObject is IDbEntity dbEntity && args.Options.PostIdAsFixed)
                getPost.AddFixedData(nameof(dbEntity.Id), dbEntity.Id);
            if (args.Options.PostObjectAsDefault)
                getPost.AddDefaultData(args.ClassObject);
            if(getPost.GetVariableData == null)
                getPost.GetVariableData = form.TriggerGetValue();
        }

        var newCC = new UICCallCollection(UICGeneratorPropertyCallType.ClassObject, form, args.CallCollection);
        form.Add(await args.Configuration.GetChildComponentAsync(args.ClassObject, args.PropertyInfo, args.Options, newCC));
        

        form.Parent = args.CallCollection.Caller;

        var toolbarCC = new UICCallCollection(UICGeneratorPropertyCallType.ButtonToolbar, form, args.CallCollection);
        var toolbar =await args.Configuration.GetChildComponentAsync(args.ClassObject, args.PropertyInfo, args.Options, toolbarCC);
        if (form.Children.FirstOrDefault()?.GetType().IsAssignableTo(typeof(UICCard)) ?? false)
           form.FindFirstOfType<UICCard>().AddFooter(toolbar);
        else
            form.Add(toolbar);


        return GeneratorHelper.Success<IUIComponent>(form, true);

    }
}
