
using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputMultiline : UICGeneratorProperty
{
    private readonly IUICValidationService _validationService;
    public UICGeneratorInputMultiline(ILogger<UICGeneratorInputMultiline> logger, IUICValidationService validationService) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        UICPropertyType = Abstractions.Attributes.UICPropertyType.MultilineText;
        HasExistingResult = false;
        _validationService = validationService;
    }
    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        var input = new UICInputMultiline(args.PropertyName)
        {
            Parent = args.CallCollection.Caller
        };
        input.Value = args.PropertyValue?.ToString()?? string.Empty;

        if (args.Configuration.TryGetPermissionService(out var permissionService))
            input.Readonly = !(await permissionService!.CanEditObject(args.ClassObject!) && await permissionService.CanEditProperty(args.ClassObject, args.PropertyName));



        input.ValidationRequired = await _validationService.ValidatePropertyRequired(args.PropertyInfo, args.ClassObject);
        input.ValidationMinLength = await _validationService.ValidatePropertyMinLength(args.PropertyInfo, args.ClassObject);
        input.ValidationMaxLength = await _validationService.ValidatePropertyMaxLength(args.PropertyInfo, args.ClassObject);

        return GeneratorHelper.Success<IUIComponent>(input, true);

    }
}
