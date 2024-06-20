using System.Collections;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.Property;

/// <summary>
/// Generate selectlistitems for enums
/// </summary>
public class UICGeneratorEnumSelectListItems : UICGeneratorBase<UICPropertyArgs, List<UICSelectListItem>>
{


    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<List<UICSelectListItem>>> GetResponseAsync(UICPropertyArgs args, List<UICSelectListItem>? existingResult)
    {
        if(args.CallCollection.CurrentCallType != UICGeneratorPropertyCallType.SelectListItems)
            return GeneratorHelper.Next<List<UICSelectListItem>>();

        bool isNullable = args.PropertyType.IsGenericType && args.PropertyType!.GetGenericTypeDefinition() == typeof(Nullable<>);

        var enumType = Nullable.GetUnderlyingType(args.PropertyType!) ?? args.PropertyType!;
        if (enumType.IsArray)
            enumType = enumType.GetElementType();
        if (!enumType.IsEnum)
            return GeneratorHelper.Next<List<UICSelectListItem>>();

        List<UICSelectListItem> items = new();
        var enumItems = enumType.GetEnumNames();

        foreach(var item in enumItems)
        {
            int value = (int)Enum.Parse(enumType, item);
            string text = item;
            if (args.Configuration.TryGetLanguageService(out var languageService))
            {
                var translateable = TranslationDefaults.TranslateEnums(enumType, item);
                text = await languageService!.Translate(translateable);
            }
            items.Add(new SelectListItem()
            {
                Value = value.ToString(),
                Text = text
            });
        }
        return GeneratorHelper.Success<List<UICSelectListItem>>(items, true);
    }
}
