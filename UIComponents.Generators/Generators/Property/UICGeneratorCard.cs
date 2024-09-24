using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Helpers;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Generators.Generators.Property.Inputs;
using UIComponents.Generators.Helpers;
using UIComponents.Models.Models.Card;

namespace UIComponents.Generators.Generators.Property;

public class UICGeneratorCard : UICGeneratorProperty
{
    private readonly IUICLanguageService _languageService;
    public UICGeneratorCard(ILogger<UICGeneratorInputThreeStateBool> logger, IUICLanguageService uICLanguageService) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.ClassObject;
        HasExistingResult = false;
        _languageService = uICLanguageService;
    }

    public override double Priority { get; set; } = 0;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.CallCollection.Caller is UICCard)
            return GeneratorHelper.Next();

        if (args.CallCollection.Caller is UICForm && !args.Options.FormToolbarInCardFooter)
            return GeneratorHelper.Next();

        UICCard card = null;
        if (args.CallCollection.Caller == null && !args.CallCollection.Components.Any() && args.Options.StartInCard != null)
            card = InternalHelper.CloneObject(args.Options.StartInCard, true);
        else if (args.CallCollection.Components.Any(x=>x is not UICPartial) && args.Options.SubClassesInCard != null)
        {
            card = InternalHelper.CloneObject(args.Options.SubClassesInCard, true);
            card.Body = new();
            card.Footer = InternalHelper.CloneObject(args.Options.SubClassesInCard.Footer, false);
            card.Header = InternalHelper.CloneObject(args.Options.SubClassesInCard.Header, false);
            card.RenderConditions.Clear();
            card.RenderConditions.Add(card.Body.HasValue);

            if (card.Header == null)
                card.Header = new UICCardHeader();
            switch (args.Options.SubCardTitleOverride)
            {
                case CardTitleOverride.NoOverride:
                    break;
                case CardTitleOverride.ClassTranslatedNameOrTostring:
                    card.Header.Title = await _languageService.TranslateObject(args.ClassObject);
                    break;
                case CardTitleOverride.ClassToString:
                    card.Header.Title = args.ClassObject.ToString();
                    break;
                case CardTitleOverride.ClassType:
                    card.Header.Title = Defaults.TranslationDefaults.TranslateType(args.ClassObject.GetType());
                    break;
                case CardTitleOverride.PropertyName:
                    if(args.PropertyInfo != null)
                        card.Header.Title = Defaults.TranslationDefaults.TranslateProperty(args.PropertyInfo, args.UICPropertyType);
                    break;
            }
        }

        if (card == null)
            return GeneratorHelper.Next();

        card.Render = true;
        card.Parent = args.CallCollection.Caller;
        card.HideHeader = !args.Options.ShowCardHeaders;


        var cc = new UICCallCollection(UICGeneratorPropertyCallType.ClassObject, card, args.CallCollection);
        var newArgs = new UICPropertyArgs(args.ClassObject, null, null, args.Options, cc, args.Configuration);
        var result = await args.Configuration.GetGeneratedResultAsync<UICPropertyArgs, IUIComponent>($"Content for card {args.ClassObject.GetType().Name}", newArgs, args.Options);
        card.Add(result);
        

        return GeneratorHelper.Success(card, true);
    }
}
