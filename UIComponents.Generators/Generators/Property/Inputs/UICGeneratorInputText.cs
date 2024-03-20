using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.InteropServices;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputText : UICGeneratorProperty
{
    public UICGeneratorInputText(ILogger<UICGeneratorInputText> logger) : base(logger)
    {
        UICPropertyType = Abstractions.Attributes.UICPropertyType.String;
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        HasExistingResult= false;
    }
    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.PropertyInfo == null)
            return GeneratorHelper.Next<IUIComponent>();

        if (args.PropertyType != typeof(string))
            return GeneratorHelper.Next<IUIComponent>();


        var input = new UICInputText(args.PropertyName)
        {
            Parent = args.CallCollection.Caller
        };
        input.Value = args.PropertyValue?.ToString()??string.Empty;


        var dataTypeAttribute = args.PropertyInfo.GetCustomAttribute<DataTypeAttribute>();
        if(dataTypeAttribute != null)
            input.Type = dataTypeAttribute.DataType;

        
        input.ValidationRequired = await args.Configuration.IsPropertyRequired(args, input) ?? false;

        return GeneratorHelper.Success<IUIComponent>(input, true);
    }
}
