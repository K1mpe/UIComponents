using UIComponents.ComponentModels.Defaults;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;
using UIComponents.Generators.Models.UICGeneratorResponses;

namespace UIComponents.Generators.Generators.Property;

public class UICGeneratorLabel : UICGeneratorProperty
{
    public UICGeneratorLabel()
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyLabel;
    }
    public override double Priority { get; set; } = 1000;



    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.PropertyInfo == null)
            return new UICGeneratorResponseNext<IUIComponent>();

        if(existingResult != null)
            return new UICGeneratorResponseNext<IUIComponent>();

        var labelText = TranslationDefaults.TranslateProperty(args.PropertyInfo, args.UICPropertyType!.Value);
        var label = new UICLabel(labelText);

        var toolTipCC = new UICCallCollection(UICGeneratorPropertyCallType.PropertyTooltip, label, args.CallCollection);
        var toolTipArgs = new UICPropertyArgs(args.ClassObject, args.PropertyInfo, args.UICPropertyType, args.Options, toolTipCC, args.Configuration);
        label.Tooltip = await args.Configuration.GetToolTipAsync(args, label);

        if (args.Options.MarkLabelsAsRequired)
        {
            label.Required = await args.Configuration.IsPropertyRequired(args, label)??false;
        }
        return GeneratorHelper.Success<IUIComponent>(label, true);
    }
}
