using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;
using UIComponents.Models.Abstract;
using UIComponents.Models.Extensions;
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

        if(!args.CallCollection.Components.Any(x=>x.GetType().IsAssignableTo(typeof(UICForm))))
            return GeneratorHelper.Next<IUIComponent>();

        if (args.Configuration.TryGetPermissionService(out var permissionService))
            if (!args.Options.ReplaceSaveButtonWithCreateButton && !await permissionService.CanEdit(args.ClassObject!))
                return GeneratorHelper.Next<IUIComponent>();

        var form = new UICForm();
        var newCC = new UICCallCollection(UICGeneratorPropertyCallType.ClassObject, form, args.CallCollection);
        form.Add(await args.Configuration.GetChildComponentAsync(args.ClassObject, args.PropertyInfo, args.Options, newCC));
        form.Parent = args.CallCollection.Caller;

        form.Add(out var toolbar, new UICButtonToolbar() { Distance = args.Options.ButtonDistance });

        List<IUIComponent?> buttons = new();
        if (args.Options.ReplaceSaveButtonWithCreateButton)
        {
            var createButton = await args.Configuration.ButtonGenerators.GenerateCreateButton(args, toolbar);
            buttons.Add(createButton);
            if (args.Options.DisableSaveButtonOnValidationErrors && createButton is UIComponent createButton2)
                form.Add(new UICActionDisableSaveButtonOnValidationErrors(createButton2, form));
        }
        else
        {
            var saveButton = await args.Configuration.ButtonGenerators.GenerateSaveButton(args, toolbar);
            buttons.Add(saveButton);
            if (args.Options.DisableSaveButtonOnValidationErrors && saveButton is UIComponent saveButton2)
                form.Add(new UICActionDisableSaveButtonOnValidationErrors(saveButton2, form));
        }
            

        if(args.Options.ShowCancelButton)
            buttons.Add(await args.Configuration.ButtonGenerators.GenerateCancelButton(args, toolbar));


        if (args.Options.ShowDeleteButton)
            buttons.Add(await args.Configuration.ButtonGenerators.GenerateDeleteButton(args, toolbar));

        if(args.Options.ReverseButtonOrder)
            buttons.Reverse();

        if (args.Options.ShowEditButton)
        {
            var editButton = await args.Configuration.ButtonGenerators.GenerateEditButton(args, toolbar);
            buttons.Add(editButton);
            if(editButton is UICButtonEdit editButton2 && editButton2.ButtonSetReadonly.OnClick != null)
            {
                buttons.SelectMany(button => button.FindAllChildrenOnType<UICActionCloseCard>()).ToList().ForEach(x =>
                {
                    if (x.OnFailed == null)
                        x.OnFailed = editButton2.ButtonSetReadonly.OnClick;
                });
            }
            
        }
            

        switch (args.Options.ButtonPosition)
        {
            case ButtonPosition.Left:
                foreach (var button in buttons)
                    toolbar.AddLeft(button);
                break;
            case ButtonPosition.Center:
                foreach (var button in buttons)
                    toolbar.AddCenter(button);
                break;
            case ButtonPosition.Right:
                foreach (var button in buttons)
                    toolbar.AddRight(button);
                break;
            default:
                throw new NotImplementedException();
        }

        return GeneratorHelper.Success<IUIComponent>(form, true);

    }
}
