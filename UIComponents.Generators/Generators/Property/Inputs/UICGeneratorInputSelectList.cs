
using UIComponents.Abstractions.Extensions;
using UIComponents.Generators.Helpers;
using UIComponents.Models.Extensions;

namespace UIComponents.Generators.Generators.Property.Inputs;

public class UICGeneratorInputSelectList : UICGeneratorProperty
{
    public UICGeneratorInputSelectList()
    {
        UICPropertyType = Abstractions.Attributes.UICPropertyType.SelectList;
        HasExistingResult = false;
        RequiredCaller = UICGeneratorPropertyCallType.PropertyInput;
    }

    public override double Priority { get; set; } = 1000;
    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        bool showButtonAdd = args.Options.SelectListShowAddButtonIfAllowed;
        var input = new UICInputSelectList(args.PropertyName, new());
        input.Value = args.PropertyValue == null ? null : args.PropertyValue!.ToString();
        if(args.PropertyType.IsEnum && args.PropertyValue != null)
            input.Value = ((int)args.PropertyValue).ToString();

        input.ValidationRequired = await args.Configuration.IsPropertyRequired(args, input)?? true;
        input.SelectListItems = (await args.Configuration.GetSelectListItems(args, input)).ToUIC()?? new();

        if(!input.ValidationRequired && input.SelectListItems.Where(x => string.IsNullOrEmpty(x.Value?.ToString()??null)).Any())
            input.SelectListItems.Insert(0, new());


        if(input.Placeholder == null)
            input.Placeholder = new Translatable("Select.PlaceHolder", "Select a {0}", TranslationDefaults.TranslateType(args.PropertyType));
        
        if (args.PropertyType.IsEnum || args.CallCollection.Caller is not UICInputGroup)
            showButtonAdd = false;

        if(showButtonAdd && args.Configuration.TryGetPermissionService(out var permissionService))
        {
            var propertyType = args.PropertyType.BaseType ?? args.PropertyType;
            showButtonAdd = await permissionService!.CanCreateType(propertyType);

            if (showButtonAdd && args.CallCollection.Caller is UICInputGroup inputGroup)
            {
                inputGroup.AppendInput.Add(new UICButton()
                {
                    AppendButtonIcon = IconDefaults.Add,
                    Tooltip = new("Button.CreateOfType.Tooltip", "Create a new {0}", TranslationDefaults.TranslateType(propertyType)),
                    OnClick = new UICActionGetPost(UICActionGetPost.ActionTypeEnum.Get, propertyType.Name, "create")
                    {
                        OnSuccess = new UICActionOpenResultAsModal()
                    }
                }.AddClass("hidden-readonly"));
        }
        }
        

        input.SearchableIfMinimimResults = args.Options.SelectlistSearableForItems;

        return GeneratorHelper.Success<IUIComponent>(input, true);
        
    }
}
