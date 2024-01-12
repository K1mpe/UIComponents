using UIComponents.Generators.Helpers;
using UIComponents.Models.Defaults;
using UIComponents.Models.Extensions;
using UIComponents.Models.Models;

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
            return GeneratorHelper.Next<IUIComponent>();

        var card = new UICCard();
        if (args.Options.ShowCardHeaders)
            card.Title = TranslationDefaults.TranslateObject(args.ClassObject);


        var cc = new UICCallCollection(UICGeneratorPropertyCallType.ClassObject, card, args.CallCollection);
        var newArgs = new UICPropertyArgs(args.ClassObject, null, null, args.Options, cc, args.Configuration);
        var result = await args.Configuration.GetGeneratedResultAsync<UICPropertyArgs, IUIComponent>($"Content for card {args.ClassObject.GetType().Name}", newArgs, args.Options);
        card.Add(result);

        return GeneratorHelper.Success<IUIComponent>(card, true);
    }
}
