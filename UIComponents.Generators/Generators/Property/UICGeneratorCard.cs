using Microsoft.Extensions.Logging;
using UIComponents.Abstractions.Helpers;
using UIComponents.Generators.Generators.Property.Inputs;
using UIComponents.Generators.Helpers;
using UIComponents.Models.Models.Card;

namespace UIComponents.Generators.Generators.Property;

public class UICGeneratorCard : UICGeneratorProperty
{
    public UICGeneratorCard(ILogger<UICGeneratorInputThreeStateBool> logger) : base(logger)
    {
        RequiredCaller = UICGeneratorPropertyCallType.ClassObject;
        HasExistingResult = false;
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
        }
            
        if (card == null)
            return GeneratorHelper.Next();

        card.Parent = args.CallCollection.Caller;
        if (args.Options.ShowCardHeaders)
            card.Header = new UICCardHeader(TranslationDefaults.TranslateObject(args.ClassObject));


        var cc = new UICCallCollection(UICGeneratorPropertyCallType.ClassObject, card, args.CallCollection);
        var newArgs = new UICPropertyArgs(args.ClassObject, null, null, args.Options, cc, args.Configuration);
        var result = await args.Configuration.GetGeneratedResultAsync<UICPropertyArgs, IUIComponent>($"Content for card {args.ClassObject.GetType().Name}", newArgs, args.Options);
        card.Add(result);
        

        return GeneratorHelper.Success(card, true);
    }
}
