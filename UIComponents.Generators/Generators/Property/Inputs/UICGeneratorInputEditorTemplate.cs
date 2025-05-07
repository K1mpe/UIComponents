
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputEditorTemplate : UICGeneratorProperty
{
    public UICGeneratorInputEditorTemplate(ILogger<UICGeneratorInputEditorTemplate> logger) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
    }
    public override double Priority { get; set; } = 1;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        await Task.Delay(0);
        if (args.PropertyInfo == null)
            return GeneratorHelper.Next();

        var uiHintAttr = args.PropertyInfo.GetCustomAttribute<UIHintAttribute>();
        if (uiHintAttr == null)
            return GeneratorHelper.Next();

        var editorTemplate = new UICInputEditorTemplate()
        {
            PropertyName = args.PropertyName,
            TemplateFor = uiHintAttr.UIHint,
            AdditionalData = uiHintAttr.ControlParameters
        };
        return GeneratorHelper.Success(editorTemplate, false);
    }
}
