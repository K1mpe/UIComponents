using System.ComponentModel;
using UIComponents.Generators.Helpers;
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

        var label = new UICLabel();

        var displayNameAttr = args.PropertyInfo.GetCustomAttribute<DisplayNameAttribute>();
        bool hasInherit = UICInheritAttribute.TryGetInheritPropertyInfo(args.PropertyInfo, out var inheritPropInfo);

        if(hasInherit && displayNameAttr == null) 
        {
            displayNameAttr = inheritPropInfo.GetCustomAttribute<DisplayNameAttribute>();
        }

        if (displayNameAttr != null)
        {
            label.LabelText = displayNameAttr.DisplayName;
        }
        else
        {
            label.LabelText =  TranslationDefaults.TranslateProperty(inheritPropInfo, args.UICPropertyType!.Value);
        }

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
