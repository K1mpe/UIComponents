using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

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
        var input = new UICInputSelectlist(args.PropertyName, new());
        input.Value = args.PropertyValue == null ? null : args.PropertyValue!.ToString();
        input.ValidationRequired = await args.Configuration.IsPropertyRequired(args, input)?? true;
        input.Placeholder = new Translatable("Select.PlaceHolder", "Select a {0}", TranslationDefaults.TranslateType(args.PropertyType));
        input.SelectListItems = await args.Configuration.GetSelectListItems(args, input)?? new();

        input.SearchableIfMinimimResults = args.Options.SelectlistSearableForItems;

        return GeneratorHelper.Success<IUIComponent>(input, true);
        
    }
}
