using System.Web.Mvc;
using UIComponents.ComponentModels.Defaults;
using UIComponents.Generators.Helpers;
using UIComponents.Generators.Interfaces;

namespace UIComponents.Generators.Generators.Property;

/// <summary>
/// Generate selectlistitems for enums
/// </summary>
public class UICGeneratorEnumSelectListItems : UICGeneratorBase<UICPropertyArgs, List<SelectListItem>>
{


    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<List<SelectListItem>>> GetResponseAsync(UICPropertyArgs args, List<SelectListItem>? existingResult)
    {
        if(args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.SelectListItems)
            return GeneratorHelper.Next<List<SelectListItem>>();

        if (!args.PropertyType!.IsEnum)
            return GeneratorHelper.Next<List<SelectListItem>>();

        var type = args.PropertyType!;
        List<SelectListItem> items = new();
        var enumItems = args.PropertyType.GetEnumNames();
        foreach(var item in enumItems)
        {
            int value = (int)Enum.Parse(type, item);
            string text = item;
            if (args.Configuration.TryGetLanguageService(out var languageService))
            {
                var translateable = TranslationDefaults.TranslateEnums(type, item);
                text = await languageService!.Translate(translateable);
            }
            items.Add(new SelectListItem()
            {
                Value = value.ToString(),
                Text = text
            });
        }
        return GeneratorHelper.Success<List<SelectListItem>>(items, true);
    }
}
