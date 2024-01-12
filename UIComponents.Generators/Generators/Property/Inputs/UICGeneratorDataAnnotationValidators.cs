using System.ComponentModel.DataAnnotations;
using System.Reflection;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;
using UIComponents.Models.Models.Inputs;

namespace UIComponents.Generators.Generators.Property.Inputs;

/// <summary>
/// Use the DataAnnotationAttributes to set validation rules
/// </summary>
public class UICGeneratorDataAnnotationValidators : UICGeneratorProperty
{
    public UICGeneratorDataAnnotationValidators()
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
    }
    public override double Priority { get; set; } = 1005;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (existingResult == null)
            return GeneratorHelper.Next<IUIComponent>();

        if (args.PropertyInfo == null)
            return GeneratorHelper.Next<IUIComponent>();

        if(existingResult is UICInputText inputText)
        {
            if(inputText.ValidationMinLength == null)
            {

                var minLengthAttr = args.PropertyInfo.GetCustomAttribute<MinLengthAttribute>();
                if (minLengthAttr != null)
                    inputText.ValidationMinLength = minLengthAttr.Length;
               
            }
            if(inputText.ValidationMaxLength == null)
            {
                var maxLengthAttr = args.PropertyInfo.GetCustomAttribute<MaxLengthAttribute>();
                if (maxLengthAttr != null)
                    inputText.ValidationMaxLength = maxLengthAttr.Length;
            }
        }
        if (existingResult is UICInputMultiline inputMultiline)
        {
            if (inputMultiline.ValidationMinLength == null)
            {

                var minLengthAttr = args.PropertyInfo.GetCustomAttribute<MinLengthAttribute>();
                if (minLengthAttr != null)
                    inputMultiline.ValidationMinLength = minLengthAttr.Length;

            }
            if (inputMultiline.ValidationMaxLength == null)
            {
                var maxLengthAttr = args.PropertyInfo.GetCustomAttribute<MaxLengthAttribute>();
                if (maxLengthAttr != null)
                    inputMultiline.ValidationMaxLength = maxLengthAttr.Length;
            }
        }







        return GeneratorHelper.Next<IUIComponent>();
    }
}
