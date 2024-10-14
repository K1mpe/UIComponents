
using Microsoft.Extensions.Logging;
using System.Collections;
using UIComponents.Abstractions.Helpers;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces.ValidationRules;
using UIComponents.Abstractions.Varia;
using UIComponents.Generators.Helpers;
using UIComponents.Models.Extensions;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputSelectList : UICGeneratorProperty
{

    private readonly IUICValidationService _validationService;
    public UICGeneratorInputSelectList(ILogger<UICGeneratorInputSelectList> logger, IUICValidationService validationService) : base(logger)
    {
        UICPropertyType = Abstractions.Attributes.UICPropertyType.SelectList;
        HasExistingResult = false;
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
        _validationService = validationService;
    }

    public override double Priority { get; set; } = 1000;
    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.PropertyType.IsAssignableTo(typeof(IEnumerable)))
            return GeneratorHelper.Next();

        bool showButtonAdd = args.Options.SelectListShowAddButtonIfAllowed;
        var input = new UICInputSelectList(args.PropertyName)
        {
            Parent = args.CallCollection.Caller
        };
        input.Value = args.PropertyValue == null ? null : args.PropertyValue!.ToString();
        if(args.PropertyType.GetNullableType().IsEnum && args.PropertyValue != null)
            input.Value = ((int)args.PropertyValue).ToString();

        input.ValidationRequired = await _validationService.ValidatePropertyRequired(args.PropertyInfo, args.ClassObject);
        input.SelectListItems = (await args.Configuration.GetSelectListItems(args, input))?? new();

        if((args.Options.SelectlistAddEmptyItem ||!input.ValidationRequired) && !input.SelectListItems.Where(x => string.IsNullOrEmpty(x.Value?.ToString()??null)).Any())
            input.SelectListItems.Insert(0, new());


        if(input.Placeholder == null)
            input.Placeholder = TranslatableSaver.Save("Select.PlaceHolder", "Select a {0}", TranslationDefaults.TranslateType(args.PropertyType));
        
        if (args.PropertyType.IsEnum || args.CallCollection.Caller is not UICInputGroup)
            showButtonAdd = false;

        if(showButtonAdd && args.Configuration.TryGetPermissionService(out var permissionService))
        {
            var propertyType = await args.Configuration.GetForeignKeyTypeAsync(args.PropertyInfo, args.Options);
            if(propertyType != null)
            {
                showButtonAdd = await permissionService!.CanCreateType(propertyType);

                if (showButtonAdd && args.CallCollection.Caller is UICInputGroup inputGroup)
                {
                    inputGroup.AppendInput.Add(new UICButton()
                    {
                        AppendButtonIcon = IconDefaults.Add,
                        Tooltip = TranslatableSaver.Save("Button.CreateOfType.Tooltip", "Create a new {0}", TranslationDefaults.TranslateType(propertyType)),
                        OnClick = new UICActionGetPost(UICActionGetPost.ActionTypeEnum.Get, propertyType.Name, "create")
                        {
                            OnSuccess = new UICActionOpenResultAsModal()
                        }.AddDefaultData("modaltitle", "")
                    }.AddClass("hidden-readonly"));
                }
            }
            
        }
        

        input.SearchableIfMinimimResults = args.Options.SelectlistSearchableForItems;

        return GeneratorHelper.Success<IUIComponent>(input, true);
        
    }
}
