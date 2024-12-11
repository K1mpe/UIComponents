using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Runtime.InteropServices;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputText : UICGeneratorProperty
{
    private readonly IUICValidationService _validationService;
    public UICGeneratorInputText(ILogger<UICGeneratorInputText> logger, IUICValidationService validationService) : base(logger)
    {
        UICPropertyType = Abstractions.Attributes.UICPropertyType.String;
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        HasExistingResult = false;
        _validationService = validationService;
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
        else if(UICInheritAttribute.TryGetInheritPropertyInfo(args.PropertyInfo, out var inherit))
        {
            dataTypeAttribute = inherit.GetCustomAttribute<DataTypeAttribute>();
            if (dataTypeAttribute != null)
                input.Type = dataTypeAttribute.DataType;
        }
        if (args.Options.CheckClientSideValidation)
        {
            input.ValidationRequired = await _validationService.ValidatePropertyRequired(args.PropertyInfo, args.ClassObject);
            input.ValidationMinLength = await _validationService.ValidatePropertyMinLength(args.PropertyInfo, args.ClassObject);
            input.ValidationMaxLength = await _validationService.ValidatePropertyMaxLength(args.PropertyInfo, args.ClassObject);
        }
            

        return GeneratorHelper.Success<IUIComponent>(input, true);
    }
}
