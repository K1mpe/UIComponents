

using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Extensions;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Models.UICGeneratorResponses;

namespace UIComponents.Generators.Generators.Property;

public class UICGeneratorHtmlAttributes : UICGeneratorProperty
{
    public UICGeneratorHtmlAttributes(ILogger<UICGeneratorHtmlAttributes> logger) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyGroup;
    }
    public override double Priority { get; set; } = 10000;

    public override Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.PropertyInfo == null)
            return Task.FromResult(new UICGeneratorResponseNext<IUIComponent>() as IUICGeneratorResponse<IUIComponent>);

        if (existingResult == null)
            return Task.FromResult(new UICGeneratorResponseNext<IUIComponent>() as IUICGeneratorResponse<IUIComponent>);

        if(existingResult is UICInputGroup inputGroup)
        {
            var inputGroupAttr = args.PropertyInfo.GetInheritAttributes<UICHtmlInputGroupAttribute>();
            foreach(var attr in inputGroupAttr)
            {
                inputGroup.AddAttribute(attr.AttributeName, attr.AttributeValue.ToString());
            }
            
            var inputAttr = args.PropertyInfo.GetInheritAttributes<UICHtmlInputAttribute>();
            foreach(var attr in inputAttr)
            {
                inputGroup.Input.AddAttribute(attr.AttributeName, attr.AttributeValue.ToString());
            }

            var labelAttr = args.PropertyInfo.GetInheritAttributes<UICHtmlLabelAttribute>();
            foreach(var attr in labelAttr)
            {
                inputGroup.Label.AddAttribute(attr.AttributeName, attr.AttributeValue.ToString());
            }
        }
        return Task.FromResult(GeneratorHelper.Success(existingResult, true));
    }
}
