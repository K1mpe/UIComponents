using UIComponents.Abstractions.Extensions;
using UIComponents.Generators.Helpers;
using UIComponents.Models.Models.Card;

namespace UIComponents.Generators.Generators;

public class UICGeneratorInitialPartial : UICGeneratorProperty
{

    public UICGeneratorInitialPartial()
    {
        RequiredCaller = UICGeneratorPropertyCallType.ClassObject;
        HasExistingResult= false;
    }

    public override double Priority { get; set; } = -1;

    public override async Task<IUICGeneratorResponse<IUIComponent>> GetResponseAsync(UICPropertyArgs args, IUIComponent? existingResult)
    {
        if (args.CallCollection.Components.Any())
            return GeneratorHelper.Next();

        if (args.Options.Partial == null)
            return GeneratorHelper.Next();

        var partial = args.Options.Partial;
        partial.SkipInitialLoad = true;

        var cc = new UICCallCollection(UICGeneratorPropertyCallType.ClassObject, partial, args.CallCollection);
        var newArgs = new UICPropertyArgs(args.ClassObject, null, null, args.Options, cc, args.Configuration);
        var result = await args.Configuration.GetGeneratedResultAsync<UICPropertyArgs, IUIComponent>($"Content for Initial partial", newArgs, args.Options);


        var firstCard = result.FindFirstOnType<UICCard>();
        if(firstCard.Header != null && firstCard.Header is UICCardHeader header)
        {
            if (!header.Buttons.Any(x => x is UICButtonRefreshPartial))
                header.AddButton(new UICButtonRefreshPartial(partial));
        }

        partial.Add(result);

        return GeneratorHelper.Success(partial, true);
    }
}
