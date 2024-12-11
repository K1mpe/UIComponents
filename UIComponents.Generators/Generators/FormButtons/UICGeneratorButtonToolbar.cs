using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.FormButtons;

public class UICGeneratorButtonToolbar : UICGeneratorProperty
{
    public UICGeneratorButtonToolbar(ILogger<UICGeneratorButtonToolbar> logger) : base(logger)
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
        using (_logger.BeginScopeKvp($"{nameof(UICOptions)}.{nameof(UICOptions.ButtonOrder)}", args.Options.ButtonOrder?.ToLower()))
        {
            var buttons = (args.Options.ButtonOrder??string.Empty).ToLower().Split(",").Select(x => x.Trim()).ToList();
            if (!buttons.Contains("delete"))
                buttons.Add("delete");
            if (!buttons.Contains("cancel"))
                buttons.Add("cancel");
            if (!buttons.Contains("edit"))
                buttons.Add("edit");
            if (!buttons.Contains("save"))
                buttons.Add("save");

            var dict = args.Options.ButtonGenerators ?? new();

            foreach(var key in dict.Select(x => x.Key))
            {
                if(!buttons.Contains(key.ToLower()))
                    buttons.Add(key.ToLower());
            }
            AddFuncToKey(dict, "delete", async (toolbar, args) =>
            {
                if (!args.Options.ShowDeleteButton)
                    return;
                var deleteButton = await args.Configuration.ButtonGenerators.GenerateDeleteButton(args, toolbar);
                if (deleteButton == null)
                    return;
                var position = args.Options.DeleteButtonPosition ?? args.Options.ButtonPosition ?? toolbar.DefaultPosition;
                toolbar.Add(deleteButton, position);
            });

            AddFuncToKey(dict, "cancel", async (toolbar, args) =>
            {

                if (!args.Options.ShowCancelButton)
                    return;

                var cancelButton = await args.Configuration.ButtonGenerators.GenerateCancelButton(args, toolbar);
                if (cancelButton == null)
                    return;
                var position = args.Options.CancelButtonPosition ?? args.Options.ButtonPosition ?? toolbar.DefaultPosition;
                toolbar.Add(cancelButton, position);
            });

            AddFuncToKey(dict, "edit", async (toolbar, args) =>
            {
                if (!args.Options.ShowEditButton)
                    return;

                var editButton = await args.Configuration.ButtonGenerators.GenerateEditButton(args, toolbar);
                if (editButton == null)
                    return;
                var position = args.Options.EditButtonPosition ?? args.Options.ButtonPosition ?? toolbar.DefaultPosition;
                toolbar.Add(editButton, position);

                if (editButton is UICButtonEdit editButton2 && editButton2.ButtonSetReadonly.OnClick != null)
                {
                    var form = args.CallCollection.Components.Where(x => x.GetType().IsAssignableTo(typeof(UICForm))).OfType<UICForm>().FirstOrDefault();
                    if (form == null)
                        return;
                    form.AddScript(true, new UICCustom($"$('#{form.GetId()}').on('uic-afterSubmit', (ev, result) => {{ if(result == false)return; uic.form.readonly('#{form.GetId()}');}});"));
                }
            });

            AddFuncToKey(dict, "save", async (toolbar, args) =>
            {
                IUIComponent saveButton = null;
                if (args.Options.ReplaceSaveButtonWithCreateButton)
                    saveButton = await args.Configuration.ButtonGenerators.GenerateCreateButton(args, toolbar);
                else
                    saveButton = await args.Configuration.ButtonGenerators.GenerateSaveButton(args, toolbar);

                var position = args.Options.SaveButtonPosition ?? args.Options.ButtonPosition ?? toolbar.DefaultPosition;
                toolbar.Add(saveButton, position);

                if (args.Options.DisableSaveButtonOnValidationErrors && saveButton is UIComponent saveButton2)
                    form.AddScriptDocReady(new UICActionDisableSaveButtonOnValidationErrors(saveButton2, form));
            });

            foreach (var button in buttons)
            {
                if (string.IsNullOrWhiteSpace(button))
                    continue;
                using (_logger.BeginScopeKvp($"{nameof(UICOptions)}.{nameof(UICOptions.ButtonOrder)}.part", button))
                {
                    try
                    {
                        if (dict.TryGetValue(button, out var func))
                            await func(toolbar, args);
                        else
                            throw new Exception($"No Generator found in {nameof(UICOptions)}.{nameof(UICOptions.ButtonGenerators)} that matches this key: {button}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, ex.Message);
                    }
                }
            }
        }


        return GeneratorHelper.Success(toolbar, true);

        void AddFuncToKey<T>(Dictionary<string, T> dict, string key, T func)
        {
            if (dict.Select(x => x.Key.ToLower()).Contains(key.ToLower()))
                return;
            dict.Add(key, func);
        }
    }
}
