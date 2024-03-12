using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.FormButtons;

public class UICGeneratorButtonToolbar : UICGeneratorProperty
{
    public UICGeneratorButtonToolbar()
    {
        RequiredCaller = UICGeneratorPropertyCallType.ButtonToolbar;
        HasExistingResult = false;
    }

    public override double Priority { get; set; }

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        var form = args.CallCollection.Components.Where(x => x is UICForm).OfType<UICForm>().FirstOrDefault();
        if (form == null)
            return GeneratorHelper.Next();

        var toolbar = new UICButtonToolbar() { Distance = args.Options.ButtonDistance };

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

        if (args.Options.ShowEditButton)
        {
            var editButton = await args.Configuration.ButtonGenerators.GenerateEditButton(args, toolbar);
            buttons.Add(editButton);
            if (editButton is UICButtonEdit editButton2 && editButton2.ButtonSetReadonly.OnClick != null)
            {
                buttons.SelectMany(button => button.FindAllChildrenOfType<UICActionCloseModal>()).ToList().ForEach(x =>
                {
                    if (x.OnFailed == null)
                        x.OnFailed = editButton2.ButtonSetReadonly.OnClick;
                });
            }

        }

        if (args.Options.ShowDeleteButton)
            buttons.Add(await args.Configuration.ButtonGenerators.GenerateDeleteButton(args, toolbar));

        if (args.Options.ShowCancelButton)
            buttons.Add(await args.Configuration.ButtonGenerators.GenerateCancelButton(args, toolbar));


        
        if (args.Options.ReverseButtonOrder)
            buttons.Reverse();



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

        return GeneratorHelper.Success(toolbar, true);
    }
}
