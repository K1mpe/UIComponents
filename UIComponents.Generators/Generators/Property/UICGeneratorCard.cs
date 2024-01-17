using UIComponents.Abstractions.Extensions;
using UIComponents.Generators.Helpers;
using UIComponents.Models.Models.Card;

namespace UIComponents.Generators.Generators.Property;

public class UICGeneratorCard : UICGeneratorProperty
{
    public UICGeneratorCard()
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
        if (args.CallCollection.Caller == null && args.Options.StartInCard != null)
            card = CommonHelper.CopyObject(args.Options.StartInCard);
        else if (args.CallCollection.Caller != null && args.Options.SubClassesInCard != null)
            card = CommonHelper.CopyObject(args.Options.SubClassesInCard);
        if (card == null)
            return GeneratorHelper.Next();

        
        if (args.Options.ShowCardHeaders)
            card.Header = new UICCardHeader(TranslationDefaults.TranslateObject(args.ClassObject));


        var cc = new UICCallCollection(UICGeneratorPropertyCallType.ClassObject, card, args.CallCollection);
        var newArgs = new UICPropertyArgs(args.ClassObject, null, null, args.Options, cc, args.Configuration);
        var result = await args.Configuration.GetGeneratedResultAsync<UICPropertyArgs, IUIComponent>($"Content for card {args.ClassObject.GetType().Name}", newArgs, args.Options);
        card.Add(result);
        

        return GeneratorHelper.Success(card, true);
    }
}
