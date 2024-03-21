using Microsoft.Extensions.Logging;
using System.ComponentModel;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Generators.Generators.Property.Inputs;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Models.UICGeneratorResponses;

namespace UIComponents.Generators.Generators.Property;

public class UICGeneratorLabel : UICGeneratorProperty
{

    private readonly IUICValidationService _validationService;
    public UICGeneratorLabel(ILogger<UICGeneratorInputThreeStateBool> logger, IUICValidationService validationService) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyLabel;
        _validationService = validationService;
    }
    public override double Priority { get; set; } = 1000;



    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.PropertyInfo == null)
            return new UICGeneratorResponseNext<IUIComponent>();

        if(existingResult != null)
            return new UICGeneratorResponseNext<IUIComponent>();

        var label = new UICLabel()
        {
            Parent = args.CallCollection.Caller
        };

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
            label.Required = await _validationService.ValidatePropertyRequired(args.PropertyInfo, args.ClassObject);
        }
        return GeneratorHelper.Success<IUIComponent>(label, true);
    }
}
