using static UIComponents.Abstractions.Varia.TranslatableSaver;

namespace UIComponents.Abstractions.Extensions;

public static class TranslatableExtensions
{
    public static async Task TranslateMissing(this List<TranslatableXmlField> translatables, string language, Func<TranslatableXmlField, Task<string?>> func)
    {
        foreach(var translatable in translatables)
        {
            if (translatable.TranslationsDict.ContainsKey(language))
                continue;

            var result = await func(translatable);
            if (result == null)
                continue;
            translatable.TranslationsList.Add(new()
            {
                Code = language,
                Translation = result
            });
        }
    }
}
